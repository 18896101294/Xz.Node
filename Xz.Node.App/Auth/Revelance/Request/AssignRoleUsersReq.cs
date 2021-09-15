using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Revelance.Request
{
    /// <summary>
    /// 角色分配用户
    /// </summary>
    public class AssignRoleUsersReq
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 用户id列表
        /// </summary>
        public string[] UserIds { get; set; }
    }

    /// <summary>
    /// 角色分配模块
    /// </summary>
    public class AssignRoleModulesReq
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 模块id列表
        /// </summary>
        public string[] ModuleIds { get; set; }
    }

    /// <summary>
    /// 角色分配菜单
    /// </summary>
    public class AssignRoleMenusReq
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 模块id
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// 菜单id列表
        /// </summary>
        public string[] MenuIds { get; set; } = new string[0];
    }
}
