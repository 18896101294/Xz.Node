using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.App.Auth.Response
{
    /// <summary>
    /// 用户资料
    /// </summary>
    public class UserProfileView
    {
        /// <summary>
        /// 构造
        /// </summary>
        public UserProfileView()
        {
            Organizations = string.Empty;
            OrganizationIds = string.Empty;
            CreateUser = string.Empty;
        }

        /// <summary>
        /// 用户映射View
        /// </summary>
        /// <param name="user"></param>
        public static implicit operator UserProfileView(Auth_UserInfo user)
        {
            return user.MapTo<UserProfileView>();
        }

        /// <summary>
        /// View映射用户
        /// </summary>
        /// <param name="view"></param>
        public static implicit operator Auth_UserInfo(UserProfileView view)
        {
            return view.MapTo<Auth_UserInfo>();
        }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        /// <returns></returns>
        public string Account { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        /// <returns></returns>
        public int Sex { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        /// <returns></returns>
        public int Status { get; set; }
        /// <summary>
        /// 组织类型
        /// </summary>
        /// <returns></returns>
        public int Type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人名字
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 所属组织名称，多个可用，分隔
        /// </summary>
        public string Organizations { get; set; }
        /// <summary>
        /// 所属组织Id
        /// </summary>
        public string OrganizationIds { get; set; }
    }
}
