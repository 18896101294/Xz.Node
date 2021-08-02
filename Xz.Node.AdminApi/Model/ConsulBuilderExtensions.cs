using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.ConsulManager.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Utilities;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// Consul 服务配置扩展
    /// </summary>
    public static class ConsulBuilderExtensions
    {
        /// <summary>
        /// Consul 服务配置扩展
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <param name="consulConfig"></param>
        /// <returns></returns>
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ConsulConfig consulConfig)
        {
            if (consulConfig == null)
            {
                return app;
            }

            var consulClient = new ConsulClient(x =>
            {
                //consul 服务地址(你要注册到哪个consul服务就填写哪个)
                x.Address = new Uri(consulConfig.ConsulAddress);
            });

            //这里其实要判断一下，如果服务已经存在了就不让注册了
            HttpHelper httpHelper = new HttpHelper(consulConfig.ConsulAddress);
            var servicesDataStr = httpHelper.Get(null, "/v1/catalog/services");
            var servicesDataDic = JsonHelper.Instance.JsonStringToKeyValuePairs(servicesDataStr);
            foreach (var item in servicesDataDic)
            {
                if (item.Key.Equals(consulConfig.ServiceName))//表示已存在consul服务
                {
                    var itemDataStr = httpHelper.Get(null, $"/v1/catalog/service/{consulConfig.ServiceName}");
                    var itemDatas = JsonHelper.Instance.Deserialize<List<ConsulServiceItemViewModel>>(itemDataStr);
                    var anyItem = itemDatas.FirstOrDefault(o => o.ServiceAddress == consulConfig.ServiceIP && o.ServicePort == consulConfig.ServicePort);
                    if (anyItem != null)//如果该服务已经注册，就跳过本次注册
                    {
                        return app;
                    }
                    break;
                }
            }

            var registration = new AgentServiceRegistration()
            {
                ID = SequentialGuidGenerator.GenerateGuid().ToString(),
                Name = consulConfig.ServiceName,//服务名
                Address = consulConfig.ServiceIP, //服务绑定IP(也就是你这个项目运行的ip地址)
                Port = consulConfig.ServicePort, //服务绑定端口(也就是你这个项目运行的端口)
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(consulConfig.RegisterSeconds),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(consulConfig.HealthCheckSeconds),//健康检查时间间隔
                    HTTP = consulConfig.ServiceHealthCheck,//健康检查地址,如果健康检查失败，一段时间后，会将注册的服务移除掉
                    Timeout = TimeSpan.FromSeconds(consulConfig.HealthCheckTimeOutSeconds)//超时时间
                }
            };
            // 服务注册
            consulClient.Agent.ServiceRegister(registration).Wait();
            // 应用程序终止时，服务取消注册
            if (consulConfig.IsEnableStop)
            {
                lifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            return app;
        }
    }
}
