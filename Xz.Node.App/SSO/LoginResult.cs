using System;
using Xz.Node.Framework.Common;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// 登录返回值
    /// </summary>
    public class LoginResult : Response<string>
    {
        /// <summary>
        /// 跳转URL
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// token过期时间
        /// </summary>
        public DateTime Expires { get; set; }
    }
}