using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Org.Response
{
    /// <summary>
    /// 根据部门ID查询部门下用户view
    /// </summary>
    public class OrgUsersView
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 业务代码
        /// </summary>
        public string BizCode { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 部门Id列表
        /// </summary>
        public List<string> OrgIds { get; set; }

        /// <summary>
        /// 部门名称列表
        /// </summary>
        public List<string> OrgNames{ get;set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
