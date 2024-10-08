﻿namespace Xz.Node.App.Auth.Role.Response
{
    /// <summary>
    /// 获取角色绑定的用户列表view
    /// </summary>
    public class RoleBindUsersView
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
    }
}
