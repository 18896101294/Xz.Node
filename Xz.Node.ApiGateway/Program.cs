using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xz.Node.Framework.Common;

namespace Xz.Node.ApiGateway
{
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
            【Runing】           :  ApiGateway
            -------------------------------------------------------------------
            【Start Time】:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", ConsoleColor.Red);
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
                   var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                   configBuilder
                       .SetBasePath(basePath) //手动指定配置文件的路径
                       .AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);//添加网关配置文件
                   var configRoot = configBuilder.Build();
                   var appId = configRoot.GetValue<string>("AppSetting:AppId");
                   hostingContext.HostingEnvironment.ApplicationName = appId;
                   hostingContext.HostingEnvironment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

                   configBuilder
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables(); //加载本地配置

                   var httpHost = configRoot["AppSetting:HttpHost"];

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
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   var configuration = ConfigHelper.GetConfigRoot();
                   var httpHost = configuration["AppSetting:HttpHost"];

                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
               });
    }
}
