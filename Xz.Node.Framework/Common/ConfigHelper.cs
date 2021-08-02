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
        public static IConfigurationRoot GetConfigRoot1()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json",
                    optional: true)
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();
            return configuration;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetConfigRoot()
        {
            var configBuilder = new ConfigurationBuilder();

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
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; //忽略异常
                    })
                    .AddEnvironmentVariables();
            }
            else
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                configBuilder
                    .SetBasePath(basePath) //手动指定配置文件的路径
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables(); //加载本地配置
            }
            var configuration = configBuilder.Build();
            return configuration;
        }
    }
}
