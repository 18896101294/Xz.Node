using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Module.Request
{
    /// <summary>
    /// 加载当前用户可访问模块的菜单入参
    /// </summary>
    public class LoadMenusReq
    {
        /// <summary>
        /// 模块id
        /// </summary>
        public string ModuleId { get; set; }
    }
}
