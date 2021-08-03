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
    /// 启动项
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 部署环境
        /// </summary>
        public IHostEnvironment Environment { get; }
        /// <summary>
        /// 配置单元
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// log4日志配置文件路径
        /// </summary>
        public string Log4netPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config", "log4net.config");

        /// <summary>
        /// 启动项构造函数
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
            //在startup中需要强制创建log4net
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddLog4Net(Log4netPath);
            });
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            //services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));

            //Ocelot网关,AddConsul是添加服务发现 AddPolly是添加网关熔断处理
            services.AddOcelot().AddConsul().AddPolly();

            //可视化监控
            //services.AddHttpReports().AddHttpTransport();

            services.AddCors();

            services.AddHttpClient();

            //数据保护DataProtection
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

            //启用日志追踪记录和异常友好提示
            app.UseLogMiddleware();

            //启用Ocelot网关
            app.UseOcelot().Wait();

            //注册consul服务
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

            //app.UseHttpReports();

            //可以访问根目录下面的静态文件
            var staticfile = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
                OnPrepareResponse = (ctx) =>
                {
                    //可以在这里为静态文件添加其他http头信息，默认添加跨域信息
                    ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                }
            };
            app.UseStaticFiles(staticfile);

            //todo:测试可以允许任意跨域，正式环境要加权限
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}
