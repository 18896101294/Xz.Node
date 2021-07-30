using System;

namespace Xz.Node.App.SSO.Response
{
    /// <summary>
    /// 登录返回model
    /// </summary>
    public class LoginResultViewModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires { get; set; }
        /// <summary>
        /// token类型
        /// </summary>
        public string Token_type { get; set; }
    }
}
