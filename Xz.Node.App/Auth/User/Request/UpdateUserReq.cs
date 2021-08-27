using Xz.Node.Framework.Extensions;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.App.Auth.User.Request
{
    /// <summary>
    /// 用户添加或修改入参
    /// </summary>
    public class UpdateUserReq
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string BizCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        /// <returns></returns>
        public int Status { get; set; }
        /// <summary>
        /// 所属部门Id，多个可用，分隔
        /// </summary>
        public string OrgIds { get; set; }
        /// <summary>
        /// 所属角色Id，多个可用，分隔
        /// </summary>
        public string RoleIds { get; set; }
        /// <summary>
        /// 修改映射
        /// </summary>
        /// <param name="user"></param>
        public static implicit operator UpdateUserReq(Auth_UserInfo user)
        {
            return user.MapTo<UpdateUserReq>();
        }
        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="view"></param>
        public static implicit operator Auth_UserInfo(UpdateUserReq view)
        {
            return view.MapTo<Auth_UserInfo>();
        }
    }
}
