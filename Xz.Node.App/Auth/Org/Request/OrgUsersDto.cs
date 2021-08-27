using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.App.Base;

namespace Xz.Node.App.Auth.Org.Request
{
    /// <summary>
    /// 获取部门用户入参
    /// </summary>
    public class OrgUsersDto: PageReq
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public string OrgId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int? Status { get; set; }
    }
}
