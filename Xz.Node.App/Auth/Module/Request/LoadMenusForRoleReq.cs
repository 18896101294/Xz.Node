using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Module.Request
{
    /// <summary>
    /// 加载角色模块下的菜单入参
    /// </summary>
    public class LoadMenusForRoleReq
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 模块Id
        /// </summary>
        public List<string> ModuleIds { get; set; }
    }
}
