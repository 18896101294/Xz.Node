using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Model;
using Xz.Node.Framework.Utilities;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Domain.Test;

namespace Xz.Node.Repository
{
    /// <summary>
    /// 数据库连接上下文
    /// </summary>
    public partial class XzDbContext : DbContext
    {
        private readonly ILoggerFactory _LoggerFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 数据库连接上下文构造函数
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configuration"></param>
        public XzDbContext(DbContextOptions<XzDbContext> options,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : base(options)
        {
            _LoggerFactory = loggerFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        /// <summary>
        /// 上下文配置
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);//允许打印参数
            optionsBuilder.UseLoggerFactory(_LoggerFactory);

            /*
             * 启用ef延迟加载功能，需要引用包：Microsoft.EntityFrameworkCore.Proxies
             * 启用后可直接查询获取到实体类的导航属性（必须是 virtual 且在可被继承的类上）如： Repository.Find(u => u.Name == name).ToList();
             * 如果不启用则可通过【Include】获取导航属性
             */
            //optionsBuilder.UseLazyLoadingProxies();

            InitTenant(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 初始化多租户信息，根据租户id调整数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        private void InitTenant(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantId = _httpContextAccessor.GetTenantId();
            string connect = _configuration.GetConnectionString(tenantId);
            if (string.IsNullOrEmpty(connect))
            {
                throw new Exception($"未能找到租户{tenantId}对应的连接字符串信息");
            }

            //这个地方如果用IOption，在单元测试的时候会获取不到AppSetting的值😅
            var dbtypes = _configuration.GetSection("AppSetting:DbTypes").GetChildren()
                .ToDictionary(x => x.Key, x => x.Value);

            var dbType = dbtypes[tenantId];
            if (dbType == Define.DBTYPE_SQLSERVER)
            {
                optionsBuilder.UseSqlServer(connect);
            }
            else if (dbType == Define.DBTYPE_MYSQL)
            {
                optionsBuilder.UseMySql(connect);
            }
            else if (dbType == Define.DBTYPE_ORACLE)
            {
                optionsBuilder.UseOracle(connect);
            }
            else
            {
                throw new Exception("AppSetting:DbTypes 数据库类型未配置");
            }
        }

        /// <summary>
        /// 可重写此方法进行：设置主外键关系、设置索引、创建Sequence、设置表级联删除等操作
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<System_DataPrivilegeRuleInfo>()
                .HasKey(c => new { c.Id });

            /*【一对一关系配置】
             * 
             *设置表和表之间一对一的关系
             *比如此处：Test_OpInfo是主体实体，Test_OcInfo是依赖实体
             */
            modelBuilder.Entity<Test_OpInfo>()
                .HasOne(a => a.TestOc)
                .WithOne(b => b.TestOp)
                .HasForeignKey<Test_OcInfo>(b => b.TestOpForeignKey);//设置依赖实体的外键关系

            modelBuilder.Entity<Test_OpInfo>()
                .HasOne(a => a.TestOa)
                .WithOne(b => b.TestOp)
                .HasForeignKey<Test_OaInfo>(b => b.TestOpForeignKey);

            /*【一对多关系配置】
            * 
            *设置表和表之间一对多的关系
            *比如此处：Test_ObInfo是依赖实体主体实体，Test_OpInfo是主体实体
            *Test_ObInfo.TestOp 是 Test_OpInfo.Test_Obs 的反向导航属性（反之亦然）
            */
            modelBuilder.Entity<Test_ObInfo>()
                .HasOne(a => a.TestOp)
                .WithMany(b => b.Test_Obs)
                .HasForeignKey(a => a.TestOpForeignKey);

            /*【多对多关系配置】
             * EF core目前尚不支持多对多关系，没有实体类来表示联接表。 但是，可以通过包含联接表的实体类并映射两个不同的一对多关系，来表示多对多关系
             * Test_Op_OcInfo 是 Test_OpInfo、Test_OcInfo 的关联表
             * 这里在设置关系的时候 要特别注意循环引用的问题
             */
            modelBuilder.Entity<Test_On_OmInfo>()
                .HasOne<Test_OnInfo>(o => o.TestOn)
                .WithMany(o => o.Test_On_Oms)
                .HasForeignKey(o => o.TestOnKey);

            modelBuilder.Entity<Test_On_OmInfo>()
                .HasOne<Test_OmInfo>(o => o.TestOm)
                .WithMany(o => o.Test_On_Oms)
                .HasForeignKey(o => o.TestOmKey);

            //非数据库表，设置无主键
            modelBuilder.Entity<SysTableColumn>().HasNoKey();
        }


        /*数据库迁移：
         *【CODE FIRST 程序包控制台执行命令】：
         * 
         * Add-Migration Init：这里的 Init 只是一个名字，表示这里是初始化。：添加迁移，Init是个描述
         * update-database Init：执行修改数据库
         * 
         *【DB FIRST 程序包控制台执行命令】:
         * 
         * Get-Help about_EntityFrameworkCore       :验证是否已安装这些工具
         * Scaffold-DbContext为 DbContext 数据库的和实体类型生成代码。 为了使 Scaffold-DbContext 生成实体类型，数据库表必须具有主键。
         * 
         * 把生成的实体和上下文都输出到Models这个文件夹如果不需要这样直接输出到当前类库中不接即可：
         *          Scaffold-DbContext "Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
         *          
         * 如果model已经生成过了，想全部覆盖的话，可以在后面加一个-force命令：
         *          Scaffold-DbContext "Server=.;Database=Food;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force
         *          
         * 更新某个表：后面加-tables 表名
         *          Scaffold-DbContext "Server=.;Database=Food;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -tables Article
         * 但是更新某个表有坑啊，如果覆盖了，那个表不会生成导航属性，而且那个山下文对象也只有那个表的内容了....暂时没有找到更好的办法...
         */

        #region 数据库表

        #region System
        /// <summary>
        /// 应用管理
        /// </summary>
        public virtual DbSet<System_ApplicationInfo> Application { get; set; }
        /// <summary>
        /// 系统授权规则
        /// </summary>
        public virtual DbSet<System_DataPrivilegeRuleInfo> DataPrivilegeRule { get; set; }
        /// <summary>
        /// 自动任务
        /// </summary>
        public virtual DbSet<System_OpenJobInfo> OpenJob { get; set; }
        /// <summary>
        /// 系统日志
        /// </summary>
        public virtual DbSet<System_SysLogInfo> SysLog { get; set; }
        #endregion

        #region Auth
        /// <summary>
        /// 模块元素表(需要权限控制的按钮)
        /// </summary>
        public virtual DbSet<Auth_ModuleElementInfo> ModuleElement { get; set; }
        /// <summary>
        /// 功能模块表
        /// </summary>
        public virtual DbSet<Auth_ModuleInfo> Module { get; set; }
        /// <summary>
        /// 组织表
        /// </summary>
        public virtual DbSet<Auth_OrgInfo> Org { get; set; }
        /// <summary>
        /// 多对多关系集中映射
        /// </summary>
        public virtual DbSet<Auth_RelevanceInfo> Relevance { get; set; }
        /// <summary>
        /// 资源表
        /// </summary>
        public virtual DbSet<Auth_ResourceInfo> Resource { get; set; }
        /// <summary>
        /// 角色表
        /// </summary>
        public virtual DbSet<Auth_RoleInfo> Role { get; set; }
        /// <summary>
        /// 用户表
        /// </summary>
        public virtual DbSet<Auth_UserInfo> User { get; set; }
        #endregion

        #region Test
        /// <summary>
        /// 测试Op
        /// </summary>
        public virtual DbSet<Test_OpInfo> Test_OpInfo { get; set; }

        /// <summary>
        /// 测试Oc
        /// </summary>
        public virtual DbSet<Test_OcInfo> Test_OcInfo { get; set; }

        /// <summary>
        /// 测试Ob
        /// </summary>
        public virtual DbSet<Test_ObInfo> Test_ObInfo { get; set; }

        /// <summary>
        /// 测试Oa
        /// </summary>
        public virtual DbSet<Test_OaInfo> Test_OaInfo { get; set; }

        /// <summary>
        /// 测试On
        /// </summary>
        public virtual DbSet<Test_OnInfo> Test_OnInfo { get; set; }

        /// <summary>
        /// 测试Om
        /// </summary>
        public virtual DbSet<Test_OmInfo> Test_OmInfo { get; set; }

        /// <summary>
        /// Test_On,Test_Om关联表
        /// </summary>
        public virtual DbSet<Test_On_OmInfo> Test_On_OmInfo { get; set; }
        #endregion

        #endregion

        #region 非数据库表
        /*
         * DbQuery 已过时，现版本可用 DbSet 直接替代
         *  [Obsolete]
         *  public virtual DbQuery<SysTableColumn> SysTableColumns { get; set; }
         *  
         * 注意：使用 DbSet 需要设置 HasNoKey()
         */
        /// <summary>
        /// 表结构
        /// </summary>
        public virtual DbSet<SysTableColumn> SysTableColumns { get; set; }
        #endregion
    }
}
