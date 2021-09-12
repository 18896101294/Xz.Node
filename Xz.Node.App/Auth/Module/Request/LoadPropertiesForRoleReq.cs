using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Module.Request
{
    /// <summary>
    /// 获取角色分配的模块下数据字段入参
    /// </summary>
    public class LoadPropertiesForRoleReq
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        public List<string> ModuleIds { get; set; }
    }
}
