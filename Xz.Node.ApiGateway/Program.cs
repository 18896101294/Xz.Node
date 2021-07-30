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
            ��Runing��           :  ApiGateway
            -------------------------------------------------------------------
            ��Start Time��:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", ConsoleColor.Red);
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
                   var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config");
                   configBuilder
                       .SetBasePath(basePath) //�ֶ�ָ�������ļ���·��
                       .AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);//������������ļ�
                   var configRoot = configBuilder.Build();
                   var appId = configRoot.GetValue<string>("AppSetting:AppId");
                   hostingContext.HostingEnvironment.ApplicationName = appId;
                   hostingContext.HostingEnvironment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

                   configBuilder
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables(); //���ر�������

                   var httpHost = configRoot["AppSetting:HttpHost"];

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
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   var configuration = ConfigHelper.GetConfigRoot();
                   var httpHost = configuration["AppSetting:HttpHost"];

                   webBuilder.UseUrls(httpHost).UseStartup<Startup>();
               });
    }
}
