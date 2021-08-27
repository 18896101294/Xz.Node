using System;
using Xz.Node.App.Base;

namespace Xz.Node.App.Auth.User.Request
{
    /// <summary>
    /// 获取分页用户信息入参
    /// </summary>
    public class LoadUsersPageReq : PageReq
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态：0.启用，1.禁用
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string OrgId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 开始创建时间
        /// </summary>
        public DateTime? BeginCreatTime { get; set; }
        /// <summary>
        /// 结束创建时间
        /// </summary>
        public DateTime? EndCreatTime { get; set; }
    }
}
