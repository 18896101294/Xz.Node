using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.User.Response
{
    /// <summary>
    /// 分页获取用户列表view
    /// </summary>
    public class LoadUsersPageView
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 账号
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
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 部门Ids
        /// </summary>
        public List<string> OrgIds { get; set; }
        /// <summary>
        /// 部门名称列表
        /// </summary>
        public List<string> OrgNames { get; set; }
        /// <summary>
        /// 角色Ids
        /// </summary>
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleNames { get; set; }
    }
}
