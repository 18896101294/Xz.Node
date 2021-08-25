using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Org.Request
{
    /// <summary>
    /// 部门删除入参
    /// </summary>
    public class OrgDeleteReq
    {
        /// <summary>
        /// 删除Id集合
        /// </summary>
        public List<string> Ids { get; set; }
    }
}
