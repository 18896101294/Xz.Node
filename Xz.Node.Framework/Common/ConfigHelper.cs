using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Winton.Extensions.Configuration.Consul;

namespace Xz.Node.Framework.Common
{
    public class ConfigHelper
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetDefaultConfigRoot()
        {
            var configBuilder = new ConfigurationBuilder();
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
            configBuilder.SetBasePath(basePath);
            configBuilder.AddJsonFile("default.json", optional: true, reloadOnChange: true);
            var defaultConfigRoot = configBuilder.Build();
            return defaultConfigRoot;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetConfigRoot()
        {
            var configBuilder = new ConfigurationBuilder();

            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
            configBuilder
                .SetBasePath(basePath);

            configBuilder
                      .AddJsonFile("default.json", optional: true, reloadOnChange: true);
            var defaultConfigRoot = configBuilder.Build();
            var consulAddress = defaultConfigRoot.GetValue<string>("ConfigSetting:ConsulAddress");
            var appId = defaultConfigRoot.GetValue<string>("ConfigSetting:AppId");

            if (!string.IsNullOrWhiteSpace(consulAddress))
            {
                configBuilder
                    .AddConsul($"{appId}/appsettings.json", options =>
                    {
                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                    })
                    .AddConsul($"System/redis.json", options =>
                    {
                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulAddress); };
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                    })
                    .AddEnvironmentVariables();
            }
            else
            {
                configBuilder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("redis.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables(); //加载本地配置
            }
            var configuration = configBuilder.Build();
            return configuration;
        }
    }
}
