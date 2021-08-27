using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.Framework.Common
{
    /// <summary>
    /// 全局常量定义
    /// </summary>
    public static class Define
    {
        /// <summary>
        /// 用户角色关联KEY
        /// </summary>
        public static string USERROLE = "UserRole";
        /// <summary>
        /// 角色资源关联KEY
        /// </summary>
        public const string ROLERESOURCE = "RoleResource";
        /// <summary>
        /// 用户机构关联KEY
        /// </summary>
        public const string USERORG = "UserOrg";
        /// <summary>
        /// 角色菜单关联KEY
        /// </summary>
        public const string ROLEELEMENT = "RoleElement";
        /// <summary>
        /// 角色模块关联KEY
        /// </summary>
        public const string ROLEMODULE = "RoleModule";
        /// <summary>
        /// 角色数据字段权限
        /// </summary>
        public const string ROLEDATAPROPERTY = "RoleDataProperty";
        /// <summary>
        /// sql server
        /// </summary>
        public const string DBTYPE_SQLSERVER = "SqlServer";
        /// <summary>
        /// mysql
        /// </summary>
        public const string DBTYPE_MYSQL = "MySql";
        /// <summary>
        /// oracle
        /// </summary>
        public const string DBTYPE_ORACLE = "Oracle";
        /// <summary>
        /// token无效
        /// </summary>
        public const int INVALID_TOKEN = 401;

        public const string TOKEN_NAME = "X-Token";

        public const string JWT_TOKEN_NAME = "Authorization";
        /// <summary>
        /// 租户Id
        /// </summary>
        public const string TENANT_ID = "tenantId";

        public const string SYSTEM_USERNAME = "System";
        public const string SYSTEM_USERPWD = "123456";
        /// <summary>
        /// 初始密码
        /// </summary>
        public const string INITIAL_PWD = "123456";

        /// <summary>
        /// 数据权限配置中，当前登录用户的key
        /// </summary>
        public const string DATAPRIVILEGE_LOGINUSER = "{loginUser}";
        /// <summary>
        /// 数据权限配置中，当前登录用户角色的key
        /// </summary>
        public const string DATAPRIVILEGE_LOGINROLE = "{loginRole}";
        /// <summary>
        /// 数据权限配置中，当前登录用户部门的key
        /// </summary>
        public const string DATAPRIVILEGE_LOGINORG = "{loginOrg}";

        public const string JOBMAPKEY = "OpenJob";

        public const string PermissionName = "Permission";
        /// <summary>
        /// 系统配置缓存key
        /// </summary>
        public const string SystemConfigurationCacheKey = "xznode_SystemConfiguration";
    }
}
