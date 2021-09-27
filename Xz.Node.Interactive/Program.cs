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
                      ���汣��         ����BUG
            -------------------------------------------
            ��Author��           :  Xz
            ��Runing��           :  Interactive
            ��StartTime��        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------", ConsoleColor.Red);
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// ϵͳ����
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, configBuilder) =>
               {
                   configBuilder.AddCommandLine(args);//���������֧��

                   var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                   configBuilder
                       .SetBasePath(basePath); //�ֶ�ָ�������ļ���·��

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
                            options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; //�����쳣
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
                        .AddEnvironmentVariables(); //���ر�������
                   }
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   //�÷�����Ҫ����Microsoft.Extensions.Logging���ƿռ�
                   logging.AddFilter("System", LogLevel.Error); //���˵�ϵͳĬ�ϵ�һЩ��־
                   logging.AddFilter("Microsoft", LogLevel.Error);//���˵�ϵͳĬ�ϵ�һЩ��־
                   logging.AddFilter("HttpReports.Transport", LogLevel.Error);//���˵�HttpReports��һЩ��־
                   //logging.AddFilter("System", LogLevel.Error); //���˵�ϵͳĬ�ϵ�һЩ��־
                   logging.ClearProviders(); //ȥ��Ĭ�ϵ���־
                   //var path = Directory.GetCurrentDirectory() + "\\log4net.config";//�������ָ�������ļ���·��,��ָ��Ĭ��Ϊ����ĸ�Ŀ¼
                   //logging.AddLog4Net();
               })
               .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //��Ĭ��ServiceProviderFactoryָ��ΪAutofacServiceProviderFactory
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   var configuration = ConfigHelper.GetDefaultConfigRoot();
                   var httpHost = configuration["ConfigSetting:HttpHost"];
                   //webBuilder.UseKestrel(options =>//����Kestrel������
                   // {
                   //     options.Listen(IPAddress.Parse("1.116.5.70"), 52600, listenOptions =>
                   //     {
                   //         //����֮ǰiis�����ɵ�pfx�ļ�·����ָ�������롡����������������������
                   //         listenOptions.UseHttps(@"C:\lan\xznode_publish\admin.xznode.club_SSL\IIS\admin52788.xznode.club.pfx", "yiy5tli3toi9r");
                   //     });
                   // }).UseStartup<Startup>();
                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
               });
    }
}
