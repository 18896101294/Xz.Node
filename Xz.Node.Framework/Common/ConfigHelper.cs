using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Xz.Node.Framework.Common
{
    public class ConfigHelper
    {
        public static IConfigurationRoot GetConfigRoot()
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
    }
}
