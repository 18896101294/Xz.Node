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
    /// �������
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ������
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
            ��Author��           :  Xz
            ��Runing��           :  AdminApi
            ��StartTime��        :  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            -------------------------------------------------------------------", ConsoleColor.Red);
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

                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
                   ConsoleHelper.WriteSuccessLine($"�ӿڷ��ʵ�ַ��{httpHost}/swagger/index.html");
               });
    }
}
