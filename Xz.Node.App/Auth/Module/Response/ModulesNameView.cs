using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Module.Response
{
    public class ModulesNameView
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
    }
}
