using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using Winton.Extensions.Configuration.Consul;
using Xz.Node.Framework.Common;

namespace Xz.Node.Interactive
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
            【Runing】           :  Interactive
            【StartTime】        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------", ConsoleColor.Red);
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
                   //webBuilder.UseKestrel(options =>//设置Kestrel服务器
                   // {
                   //     options.Listen(IPAddress.Parse("1.116.5.70"), 52600, listenOptions =>
                   //     {
                   //         //填入之前iis中生成的pfx文件路径和指定的密码　　　　　　　　　　　　
                   //         listenOptions.UseHttps(@"C:\lan\xznode_publish\admin.xznode.club_SSL\IIS\admin52788.xznode.club.pfx", "yiy5tli3toi9r");
                   //     });
                   // }).UseStartup<Startup>();
                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
               });
    }
}
