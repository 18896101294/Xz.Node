using Consul;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
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
        /// <returns></returns>
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app)
        {
            var consulClient = new ConsulClient(x =>
            {
                //consul 服务地址(你要注册到哪个consul服务就填写哪个)
                x.Address = new Uri("http://127.0.0.1:8500");
            });

            var registration = new AgentServiceRegistration()
            {
                ID = SequentialGuidGenerator.GenerateGuid().ToString(),
                Name = "AdminApi",//服务名
                Address = "127.0.0.1", //服务绑定IP(也就是你这个项目运行的ip地址)
                Port = 52789, //服务绑定端口(也就是你这个项目运行的端口)
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                    HTTP = "http://localhost:52789/api/Test/HealthCheck",//健康检查地址,如果健康检查失败，一段时间后，会将注册的服务移除掉
                    Timeout = TimeSpan.FromSeconds(5)//超时时间
                }
            };
            // 服务注册
            consulClient.Agent.ServiceRegister(registration).Wait();
            // 应用程序终止时，服务取消注册
            //lifetime.ApplicationStopping.Register(() =>
            //{
            //    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            //});
            return app;
        }
    }
}
