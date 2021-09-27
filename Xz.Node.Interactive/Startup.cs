using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Xz.Node.App;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions.AutofacManager;
using Xz.Node.Framework.Middleware;
using Xz.Node.Interactive.Hubs;
using Xz.Node.Interactive.SignalrProcess;
using Xz.Node.Repository;

namespace Xz.Node.Interactive
{
    public class Startup
    {

        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        /// <summary>
        /// log4日志配置文件路径
        /// </summary>
        public string Log4netPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "config", "log4net.config");

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //在startup中需要强制创建log4net
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddLog4Net(Log4netPath);
            });

            ILogger logger = loggerFactory.CreateLogger<Startup>();

            services.AddConnections();

            services.AddSignalR().AddJsonProtocol(options =>
            {
                //Json序列化属性名不修改大小写：
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            //允许跨域
            services.AddCors(o =>
            {
                o.AddPolicy("Everything", p =>
                {
                    p.SetIsOriginAllowed(origin => true)
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
                });
            });

            services.AddControllers();

            //services.AddControllers(options =>
            //{ 
            //    options.Filters.Add(new RequireHttpsAttribute());//所有请求都使用HTTPS
            //});

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());//所有请求都使用HTTPS
            //});

            services.AddCors();

            //在startup里面只能通过这种方式获取到appsettings里面的值，不能用IOptions
            var dbtypes = ((ConfigurationSection)Configuration.GetSection("AppSetting:DbTypes")).GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);
            var connectionString = Configuration.GetConnectionString("XzNodeDBContext");
            ConsoleHelper.WriteSuccessLine($"数据库类型：{JsonHelper.Instance.Serialize(dbtypes)}");
            ConsoleHelper.WriteSuccessLine($"连接字符串：{connectionString}");
            services.AddDbContext<XzDbContext>();//注入数据库上下文
            //services.AddDbContext<XzDbContext>(o => o.UseLazyLoadingProxies().UseSqlServer(connectionString));

            services.AddHttpClient();
            //数据保护DataProtection
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtection"]));

            this.AfterStartup(services, Configuration);
            //单例注入
            services.AddHostedService<SendTimeJob>();
        }

        /// <summary>
        /// 全局注入
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            AutofacExt.InitAutofac(builder);
        }

        /// <summary>
        /// 注入配置文件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public void AfterStartup(IServiceCollection services, IConfiguration configuration)
        {
            //系统目前已支持动态化参数配置，不应在此处进行配置注入
            //services.Configure<AppSetting>(configuration.GetSection("AppSetting"));
            services.Configure<ConsulConfig>(configuration.GetSection("Consul"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net(Log4netPath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用日志追踪记录和异常友好提示
            app.UseLogMiddleware();

            //注册consul服务
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

            app.UseFileServer();

            app.UseStaticFiles();

            //允许任意跨域
            app.UseCors("Everything");

            app.UseRouting();

            //所有请求都强制转为https
            //app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            //app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ServiceHub>("/hubs");//启用SignalR，指定连接器
            });

            //配置ServiceProvider
            AutofacContainerModule.ConfigServiceProvider(app.ApplicationServices);

            //启用SignalR，指定连接器 [Obsolete]
            //app.UseSignalR(hub => hub.MapHub<ServiceBdcHub>("/hubs"));

            app.UseRouting();
        }
    }
}
