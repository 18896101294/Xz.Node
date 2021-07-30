using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.SSO.Request
{
    /// <summary>
    /// 登录接口请求参数
    /// </summary>
    public class LoginReq
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
    }
}
