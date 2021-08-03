using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Xz.Node.App;
using Xz.Node.Framework.Common;
using Xz.Node.Repository;

namespace Xz.Node.IdentityServer
{
    public class Startup
    {
        public IHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients(Environment.IsProduction()))
                .AddProfileService<CustomProfileService>();

            services.ConfigureNonBreakingSameSiteCookies();

            services.AddCors();
            //todo:�����ʽ ������������ķ�ʽ����������ʿ���
            //  var origins = new []
            //  {
            //      "http://localhost:1803",
            //      "http://localhost:52789"
            //  };
            //  if (Environment.IsProduction())
            //  {
            //      origins = new []
            //      {
            //          "http://demo.openauth.me:1803",
            //          "http://demo.openauth.me:52789"
            //      };
            //  }
            //  services.AddCors(option=>option.AddPolicy("cors", policy =>
            //      policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(origins)));

            //ȫ���ò��Ի�������ʽ������ο�https://www.cnblogs.com/guolianyu/p/9872661.html
            //if (Environment.IsDevelopment())
            //{
            builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            //    throw new Exception("need to configure key material");
            //}

            services.AddAuthentication();

            //ӳ�������ļ�
            //services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));

            //��startup����ֻ��ͨ�����ַ�ʽ��ȡ��appsettings�����ֵ��������IOptions??
            var dbtypes = ((ConfigurationSection)Configuration.GetSection("AppSetting:DbTypes")).GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);
            var dbType = dbtypes["XzNodeDBContext"];
            if (dbType == Define.DBTYPE_SQLSERVER)
            {
                services.AddDbContext<XzDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("XzNodeDBContext")));
            }
            else if (dbType == Define.DBTYPE_MYSQL)
            {
                services.AddDbContext<XzDbContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("XzNodeDBContext")));
            }
            else if (dbType == Define.DBTYPE_ORACLE)
            {
                services.AddDbContext<XzDbContext>(options =>
                    options.UseOracle(Configuration.GetConnectionString("XzNodeDBContext")));
            }
            else
            {
                throw new Exception("���ݿ�����δ����");
            }
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            AutofacExt.InitAutofac(builder);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //ע��consul����
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

            app.UseCookiePolicy();

            //todo:���Կ����������������ʽ����Ҫ��Ȩ��
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
