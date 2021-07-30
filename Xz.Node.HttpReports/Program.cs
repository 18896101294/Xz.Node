using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.Framework.Common;

namespace Xz.Node.HttpReports
{
    public class Program
    {
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
            【Runing】           :  HttpReports
            -------------------------------------------------------------------
            【Start Time】:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", ConsoleColor.Red);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                    configBuilder
                        .SetBasePath(basePath); //手动指定配置文件的路径
                    var configRoot = configBuilder.Build();
                    var appId = configRoot.GetValue<string>("AppSetting:AppId");
                    hostingContext.HostingEnvironment.ApplicationName = appId;
                    hostingContext.HostingEnvironment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

                    configBuilder
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables(); //加载本地配置
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var configuration = ConfigHelper.GetConfigRoot();
                    var httpHost = configuration["AppSetting:HttpHost"];
                    webBuilder.UseUrls(httpHost).UseStartup<Startup>();
                });
    }
}
