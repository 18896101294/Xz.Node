using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Quartz;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Xz.Node.App.Interface;
using Xz.Node.App.SSO;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Extensions.AutofacManager;
using Xz.Node.Framework.Jwt;
using Xz.Node.Framework.Queue.RabbitMQ;
using Xz.Node.Framework.Queue.RedisQueue;
using Xz.Node.Repository;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App
{
    /// <summary>
    /// 全局注入
    /// </summary>
    public static class AutofacExt
    {
        private static IContainer _container;

        /// <summary>
        /// 单元测试
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IContainer InitForTest(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            //注册数据库基础操作和工作单元
            services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped(typeof(IUnitWork<>), typeof(UnitWork<>));

            //注入授权
            builder.RegisterType(typeof(LocalAuth)).As(typeof(IAuth));

            //注册app层
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());

            //防止单元测试时已经注入
            if (services.All(u => u.ServiceType != typeof(ICacheContext)))
            {
                services.AddScoped(typeof(ICacheContext), typeof(CacheContext));
            }

            if (services.All(u => u.ServiceType != typeof(IHttpContextAccessor)))
            {
                services.AddScoped(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));
            }

            InitDependency(builder);

            builder.RegisterModule(new QuartzAutofacFactoryModule());

            builder.Populate(services);

            _container = builder.Build();
            return _container;
        }

        /// <summary>
        /// 全局注入
        /// </summary>
        /// <param name="builder"></param>
        public static void InitAutofac(ContainerBuilder builder)
        {
            //注册数据库基础操作和工作单元
            builder.RegisterGeneric(typeof(BaseRepository<,>)).As(typeof(IRepository<,>));
            builder.RegisterGeneric(typeof(UnitWork<>)).As(typeof(IUnitWork<>));
            //注入授权
            builder.RegisterType(typeof(LocalAuth)).As(typeof(IAuth));

            //注册app层
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());

            //RedisCacheContext、EnyimMemcachedContext、CacheContext都实现ICacheContext,这里注入哪个就启用了哪个缓存
            builder.RegisterType(typeof(RedisCacheContext)).As(typeof(ICacheContext));

            //注入redis 队列
            builder.RegisterType(typeof(RedisQueueHelper)).As(typeof(IRedisQueueHelper));

            //注入RabbitMQClient
            builder.RegisterType(typeof(RabbitMQClient)).As(typeof(IRabbitMQClient));

            //注入jwt，这里因为jwt启用时需要注入，懒得去容器里面去拿了，就直接注入了
            builder.RegisterType(typeof(JwtTokenHelper)).As(typeof(IJwtTokenHelper));

            InitDependency(builder);

            builder.RegisterModule(new QuartzAutofacFactoryModule());
        }

        /// <summary>
        /// 注入所有继承了IDependency接口
        /// </summary>
        /// <param name="builder"></param>
        private static void InitDependency(ContainerBuilder builder)
        {
            Type baseType = typeof(IDependency);
            var compilationLibrary = DependencyContext.Default
                .CompileLibraries
                .Where(x => !x.Serviceable
                            && x.Type == "project")
                .ToList();
            var count1 = compilationLibrary.Count;
            List<Assembly> assemblyList = new List<Assembly>();

            foreach (var _compilation in compilationLibrary)
            {
                try
                {
                    assemblyList.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(_compilation.Name)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(_compilation.Name + ex.Message);
                }
            }

            builder.RegisterAssemblyTypes(assemblyList.ToArray())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf().AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
