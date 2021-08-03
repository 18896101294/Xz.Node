using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System;
using System.IO;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Middleware;

namespace Xz.Node.ApiGateway
{
    /// <summary>
    /// ������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ���𻷾�
        /// </summary>
        public IHostEnvironment Environment { get; }
        /// <summary>
        /// ���õ�Ԫ
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// log4��־�����ļ�·��
        /// </summary>
        public string Log4netPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config", "log4net.config");

        /// <summary>
        /// ������캯��
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //��startup����Ҫǿ�ƴ���log4net
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddLog4Net(Log4netPath);
            });
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            //services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));

            //Ocelot����,AddConsul����ӷ����� AddPolly����������۶ϴ���
            services.AddOcelot().AddConsul().AddPolly();

            //���ӻ����
            //services.AddHttpReports().AddHttpTransport();

            services.AddCors();

            services.AddHttpClient();

            //���ݱ���DataProtection
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtection"]));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net(Log4netPath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //������־׷�ټ�¼���쳣�Ѻ���ʾ
            app.UseLogMiddleware();

            //����Ocelot����
            app.UseOcelot().Wait();

            //ע��consul����
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

            //app.UseHttpReports();

            //���Է��ʸ�Ŀ¼����ľ�̬�ļ�
            var staticfile = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
                OnPrepareResponse = (ctx) =>
                {
                    //����������Ϊ��̬�ļ��������httpͷ��Ϣ��Ĭ����ӿ�����Ϣ
                    ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                }
            };
            app.UseStaticFiles(staticfile);

            //todo:���Կ����������������ʽ����Ҫ��Ȩ��
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}
