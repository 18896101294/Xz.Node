using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Winton.Extensions.Configuration.Consul;
using Xz.Node.Framework.Common;

namespace Xz.Node.AdminApi
{
    /// <summary>
    /// 程序入口
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            ConsoleHelper.WriteInfoLine($@"
               _   _   ____        _                        
              | \ | | / __ \      | |   
              |  \| || |  | |  ___| |  ___ 
              | . ` || |  | | / _ | | / _ \
              | |\  || |__| || |_)| ||  __/
              |_| \_| \____/  \___|_| \___|
                                                                           
            -------------------------------------------------------------------
            【Author】           :  Xz
            【Runing】           :  AdminApi
            【StartTime】        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------------------------------", ConsoleColor.Red);
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 系统配置
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, configBuilder) =>
               {
                   configBuilder.AddCommandLine(args);//添加命令行支持

                   var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                   configBuilder
                       .SetBasePath(basePath); //手动指定配置文件的路径

                   configBuilder
                        .AddJsonFile("default.json", optional: true, reloadOnChange: true);
                   var defaultConfigRoot = configBuilder.Build();
                   var consulAddress = defaultConfigRoot.GetValue<string>("ConfigSetting:ConsulAddress");
                   var appId = defaultConfigRoot.GetValue<string>("ConfigSetting:AppId");

                   hostingContext.HostingEnvironment.ApplicationName = appId;
                   hostingContext.HostingEnvironment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

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
                        .AddConsul($"System/consul.json", options =>
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
                        .AddJsonFile("consul.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("redis.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables(); //加载本地配置
                   }
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   //该方法需要引入Microsoft.Extensions.Logging名称空间
                   logging.AddFilter("System", LogLevel.Error); //过滤掉系统默认的一些日志
                   logging.AddFilter("Microsoft", LogLevel.Error);//过滤掉系统默认的一些日志
                   logging.AddFilter("HttpReports.Transport", LogLevel.Error);//过滤掉HttpReports的一些日志
                   //logging.AddFilter("System", LogLevel.Error); //过滤掉系统默认的一些日志
                   logging.ClearProviders(); //去掉默认的日志
                   //var path = Directory.GetCurrentDirectory() + "\\log4net.config";//这里可以指定配置文件的路径,不指定默认为程序的根目录
                   //logging.AddLog4Net();
               })
               .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //将默认ServiceProviderFactory指定为AutofacServiceProviderFactory
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   var configuration = ConfigHelper.GetDefaultConfigRoot();
                   var httpHost = configuration["ConfigSetting:HttpHost"];

                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
                   ConsoleHelper.WriteSuccessLine($"接口访问地址：{httpHost}/swagger/index.html");
               });
    }
}
