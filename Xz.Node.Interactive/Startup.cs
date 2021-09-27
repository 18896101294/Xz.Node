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
        /// log4��־�����ļ�·��
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
            //��startup����Ҫǿ�ƴ���log4net
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddLog4Net(Log4netPath);
            });

            ILogger logger = loggerFactory.CreateLogger<Startup>();

            services.AddConnections();

            services.AddSignalR().AddJsonProtocol(options =>
            {
                //Json���л����������޸Ĵ�Сд��
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            //�������
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
            //    options.Filters.Add(new RequireHttpsAttribute());//��������ʹ��HTTPS
            //});

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());//��������ʹ��HTTPS
            //});

            services.AddCors();

            //��startup����ֻ��ͨ�����ַ�ʽ��ȡ��appsettings�����ֵ��������IOptions
            var dbtypes = ((ConfigurationSection)Configuration.GetSection("AppSetting:DbTypes")).GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);
            var connectionString = Configuration.GetConnectionString("XzNodeDBContext");
            ConsoleHelper.WriteSuccessLine($"���ݿ����ͣ�{JsonHelper.Instance.Serialize(dbtypes)}");
            ConsoleHelper.WriteSuccessLine($"�����ַ�����{connectionString}");
            services.AddDbContext<XzDbContext>();//ע�����ݿ�������
            //services.AddDbContext<XzDbContext>(o => o.UseLazyLoadingProxies().UseSqlServer(connectionString));

            services.AddHttpClient();
            //���ݱ���DataProtection
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtection"]));

            this.AfterStartup(services, Configuration);
            //����ע��
            services.AddHostedService<SendTimeJob>();
        }

        /// <summary>
        /// ȫ��ע��
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            AutofacExt.InitAutofac(builder);
        }

        /// <summary>
        /// ע�������ļ�
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public void AfterStartup(IServiceCollection services, IConfiguration configuration)
        {
            //ϵͳĿǰ��֧�ֶ�̬���������ã���Ӧ�ڴ˴���������ע��
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

            //������־׷�ټ�¼���쳣�Ѻ���ʾ
            app.UseLogMiddleware();

            //ע��consul����
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

            app.UseFileServer();

            app.UseStaticFiles();

            //�����������
            app.UseCors("Everything");

            app.UseRouting();

            //��������ǿ��תΪhttps
            //app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            //app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ServiceHub>("/hubs");//����SignalR��ָ��������
            });

            //����ServiceProvider
            AutofacContainerModule.ConfigServiceProvider(app.ApplicationServices);

            //����SignalR��ָ�������� [Obsolete]
            //app.UseSignalR(hub => hub.MapHub<ServiceBdcHub>("/hubs"));

            app.UseRouting();
        }
    }
}
