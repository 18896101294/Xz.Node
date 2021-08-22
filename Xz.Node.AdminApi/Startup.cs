using Autofac;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xz.Node.AdminApi.Model;
using Xz.Node.App;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Extensions.AutofacManager;
using Xz.Node.Framework.Middleware;
using Xz.Node.Repository;

namespace Xz.Node.AdminApi
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
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
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

            #region identityServer4
            var identityServerEnabled =
               ((ConfigurationSection)Configuration.GetSection("AppSetting:IdentityServer4:Enabled")).Value.ToBool();
            var identityServer =
              ((ConfigurationSection)Configuration.GetSection("AppSetting:IdentityServer4:IdentityServerUrl")).Value;
            if (identityServerEnabled)
            {
                services.AddAuthorization();

                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    //.AddIdentityServerAuthentication(options =>
                    //{
                    //    //设置token过期得时间偏移量，默认是5分钟
                    //    options.JwtValidationClockSkew = TimeSpan.FromSeconds(0);
                    //})
                    .AddJwtBearer(options =>
                    {
                        options.Authority = identityServer;
                        options.RequireHttpsMetadata = false; // 指定是否为HTTPS
                        options.Audience = "xznodeapi";//这里的options.ApiName 需要和网关服务中的Api 资源配置中的ApiName 一致
                    });
            }
            #endregion

            #region Jwt
            var jwtEnabled = ((ConfigurationSection)Configuration.GetSection("AppSetting:Jwt:Enabled")).Value.ToBool();
            if (jwtEnabled)
            {
                //启用jwt方式鉴权
                services.AddAuthentication(Options =>
                {
                    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).
                AddJwtBearer();

                ////基于自定义策略授权
                //services.AddAuthorization(options =>
                //{
                //    options.AddPolicy(Define.PermissionName,
                //      policy => policy
                //        .Requirements
                //        .Add(new PermissionRequirement("admin")));
                //});
                ////此外，还需要在 IAuthorizationHandler 类型的范围内向 DI 系统注册新的处理程序：
                //services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            }
            #endregion

            #region 添加MiniProfiler服务
            services.AddMiniProfiler(options =>
            {
                //设定访问分析结果URL的路由基地址
                options.RouteBasePath = "/profiler";
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
                options.PopupShowTrivial = true;
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                //options.IgnoredPaths.Add("/swagger/");
            }).AddEntityFramework(); //显示SQL语句及耗时
            #endregion

            #region 添加swagger
            services.AddSwaggerGen(option =>
            {
                foreach (var controller in this.GetControllers())
                {
                    var groupname = GetSwaggerGroupName(controller);//根据控制器分组

                    option.SwaggerDoc(groupname, new OpenApiInfo
                    {
                        Version = "v1",
                        Title = controller.Name.Replace("Controller", ""),
                        Description = "by xz"
                    });
                }

                //ConsoleHelper.WriteSuccessLine($"api doc basepath:{AppContext.BaseDirectory}");

                foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.*",
                    SearchOption.AllDirectories).Where(f => Path.GetExtension(f).ToLower() == ".xml"))
                {
                    option.IncludeXmlComments(name, includeControllerXmlComments: true);//读取swagger xml文件，项目属性-》生成配置
                    //logger.LogInformation($"find api file{name}");
                }

                option.OperationFilter<GlobalHttpHeaderOperationFilter>(); // 添加httpHeader参数


                if (identityServerEnabled)
                {
                    //接入identityserver,这里的方案名称必须是oauth2
                    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Description = "identityServer4登陆授权",
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{identityServer}/connect/authorize"),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"xznodeapi", "同意xz.node.adminapi 的访问权限"} //指定客户端请求的api作用域。 如果为空，则客户端无法访问
                                }
                            }
                        }
                    });
                    option.OperationFilter<AuthResponsesOperationFilter>();
                }

                if (jwtEnabled)
                {
                    //接入jwt,这里的方案名称必须是oauth2
                    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                        Name = "Authorization",//jwt默认的参数名称
                        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey
                    });
                    // 开启加权小锁
                    //option.OperationFilter<AddResponseHeadersFilter>();
                    //option.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                    // 在header中添加token，传递到后台
                    option.OperationFilter<SecurityRequirementsOperationFilter>();

                    option.OperationFilter<AuthResponsesOperationFilter>();

                }
            });
            #endregion

            services.AddControllers(option =>
            {
                /*
                 * 这里有两种添加MVC过滤器的方法。
                 * 另外一种为：
                 *  option.Filters.Add(typeof(GloabExceptionFilterAttribute));
                 * 不能同时使用两种写法，如果同时使用两种写法目前会导致两个过滤器都不生效。
                 */
                option.Filters.Add<GloabExceptionFilterAttribute>();
                option.Filters.Add<OpenAuthFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // 禁用自动模态验证
                // options.SuppressModelStateInvalidFilter = true;

                //启动WebAPI自动模态验证，处理返回值
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problems = new CustomBadRequest(context);
                    return new BadRequestObjectResult(problems);
                };
            })
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();    
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            //可视化监控
            services.AddHttpReports().AddHttpTransport();

            services.AddMemoryCache();
            services.AddCors();
            //todo:如果正式 环境请用下面的方式限制随意访问跨域
            //var origins = new []
            //{
            //    "http://localhost:1803",
            //    "http://localhost:52789"
            //};
            //if (Environment.IsProduction())
            //{
            //    origins = new []
            //    {
            //        "http://demo.openauth.me:1803",
            //        "http://demo.openauth.me:52789"
            //    };
            //}
            //services.AddCors(option=>option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(origins)));

            //在startup里面只能通过这种方式获取到appsettings里面的值，不能用IOptions😰
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

            //设置定时启动的任务
            //services.AddHostedService<QuartzService>();

            this.AfterStartup(services, Configuration);
        }

        /// <summary>
        /// 全局注入
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            AutofacExt.InitAutofac(builder);
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
                app.UseMiniProfiler();
            }

            //启用日志追踪记录和异常友好提示
            app.UseLogMiddleware();

            app.UseHttpReports();

            //注册consul服务
            app.RegisterConsul(lifetime, ConsulHelper.GetConsulConfig(Configuration));

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

            #region 解决Ubuntu Nginx 代理不能获取IP问题
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            #endregion

            app.UseStaticFiles(staticfile);

            //todo:测试可以允许任意跨域，正式环境要加权限
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseRouting();

            app.UseAuthentication();//identityServer4身份认证的时候必须启用

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //配置ServiceProvider
            AutofacContainerModule.ConfigServiceProvider(app.ApplicationServices);

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.IndexStream = () =>
                   GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Xz.Node.AdminApi.index.html");//自定义swagger页面

                foreach (var controller in GetControllers())
                {
                    var groupname = GetSwaggerGroupName(controller);

                    c.SwaggerEndpoint($"/swagger/{groupname}/swagger.json", groupname);
                }

                c.DocExpansion(DocExpansion.List);//默认展开列表
                c.OAuthClientId("XzNode.AdminApi");//客户端名称
                c.OAuthAppName("xz.node.adminapi认证");//客户端描述
            });

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

        #region private
        /// <summary>
        /// 获取控制器对应的swagger分组值
        /// </summary>
        private string GetSwaggerGroupName(Type controller)
        {
            var groupname = controller.Name.Replace("Controller", "");
            var apisetting = controller.GetCustomAttribute(typeof(ApiExplorerSettingsAttribute));
            if (apisetting != null)
            {
                groupname = ((ApiExplorerSettingsAttribute)apisetting).GroupName;
            }

            return groupname;
        }

        /// <summary>
        /// 获取所有的控制器
        /// </summary>
        private List<Type> GetControllers()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .OrderBy(x => x.Name).ToList();
            return controlleractionlist;
        }
        #endregion
    }
}
