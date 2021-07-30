using System.ComponentModel;

namespace Xz.Node.Framework.Enums
{
    /// <summary>
    /// 系统授权方式枚举
    /// </summary>
    public enum AuthorizationWayEnum
    {
        /// <summary>
        /// IdentityServer4
        /// </summary>
        [Description("IdentityServer4")]
        IdentityServer4 = 1,
        /// <summary>
        /// jwt
        /// </summary>
        [Description("jwt")]
        Jwt = 2,
        /// <summary>
        /// OAuth2
        /// </summary>
        [Description("OAuth2")]
        OAuth2 = 3
    }
}
