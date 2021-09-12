using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Module.Request
{
    /// <summary>
    /// 获取模块数据字段入参
    /// </summary>
    public class CheckedPropertiesReq
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// 模块页面名称
        /// </summary>
        public string ModuleViewName { get; set; }
    }
}
