using System.ComponentModel;

namespace Xz.Node.Framework.Enums
{
    /// <summary>
    /// Jwt token 校验返回类型枚举
    /// </summary>
    public enum TokenResponseEnum
    {
        /// <summary>
        /// 校验成功
        /// </summary>
        [Description("校验成功")]
        Ok = 1,
        /// <summary>
        /// 校验失败
        /// </summary>
        [Description("校验失败")]
        Fail = 2,
        /// <summary>
        /// token过期
        /// </summary>
        [Description("token过期")]
        Expired = 3
    }
}
