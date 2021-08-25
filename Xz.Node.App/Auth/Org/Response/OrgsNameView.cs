using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Org.Response
{
    /// <summary>
    /// 部门下来菜单model
    /// </summary>
    public class OrgsNameView
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
    }
}
