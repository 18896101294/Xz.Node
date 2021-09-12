using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.Framework.Common;

namespace Xz.Node.App.Auth.Module.Response
{
    /// <summary>
    /// 获取勾选模块数据字段信息
    /// </summary>
    public class CheckedPropertiesView
    {
        /// <summary>
        /// 模块id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块路径名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 模块数据字典列表
        /// </summary>
        public List<KeyDescription> Keys { get; set; }
    }
}
