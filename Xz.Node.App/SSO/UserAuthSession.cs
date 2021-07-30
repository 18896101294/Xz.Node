using System;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// 用户token信息
    /// </summary>
    [Serializable]
    public class UserAuthSession
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 应用key
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}