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
                              _oo0oo_
                             o8888888o
                             88"" . ""88
                             (| -_- |)
                             0\  =  /0
                           ___/`---'\___
                         .' \\|     |// '.
                        / \\|||  :  |||// \
                       / _||||| -:- |||||- \
                      |   | \\\  -  /// |   |
                      | \_|  ''\---/''  |_/ |
                      \  .-\__  '-'  ___/-. /
                    ___'. .'  /--.--\  `. .'___
                 ."" '<  `.___\_<|>_/___.' >' "".
                | | :  `- \`.;`\ _ /`;.`/ - ` : | |
                \  \ `_.   \_ __\ /__ _/   .-` /  /
            =====`-.____`.___ \_____/___.-`___.-'=====
                              `=---='
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                      佛祖保佑         永无BUG
            -------------------------------------------
            【Author】           :  Xz
            【Runing】           :  HttpReports
            【StartTime】        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------", ConsoleColor.Red);
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
                    var configuration = ConfigHelper.GetDefaultConfigRoot();
                    var httpHost = configuration["AppSetting:HttpHost"];
                    webBuilder.UseUrls(httpHost).UseStartup<Startup>();
                });
    }
}
