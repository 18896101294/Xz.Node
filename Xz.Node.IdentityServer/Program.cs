using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using Winton.Extensions.Configuration.Consul;
using Xz.Node.Framework.Common;

namespace Xz.Node.IdentityServer
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
            【Runing】           :  IdentityServer4
            【StartTime】        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------", ConsoleColor.Red);
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    configBuilder.AddCommandLine(args);//添加命令行支持

                    var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                    configBuilder
                         .SetBasePath(basePath);

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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())   //将默认ServiceProviderFactory指定为AutofacServiceProviderFactory
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var configuration = ConfigHelper.GetDefaultConfigRoot();
                    var httpHost = configuration["ConfigSetting:HttpHost"];
                    webBuilder.UseUrls(httpHost).UseStartup<Startup>();
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                });
        }
    }
}
