using System;
using Xz.Node.Framework.Common;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// ��¼����ֵ
    /// </summary>
    public class LoginResult : Response<string>
    {
        /// <summary>
        /// ��תURL
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// token����ʱ��
        /// </summary>
        public DateTime Expires { get; set; }
    }
}