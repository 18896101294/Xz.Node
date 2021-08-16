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
}
