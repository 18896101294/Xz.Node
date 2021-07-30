using System;
using System.Collections.Generic;
using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Jwt
{
    /// <summary>
    /// Jwt token 接口
    /// </summary>
    public interface IJwtTokenHelper
    {
        /// <summary>
        /// 根据一个对象通过反射提供负载生成token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        JwtTokenResponse CreateToken<T>(T user) where T : class;
        /// <summary>
        /// 根据键值对提供负载生成token
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        JwtTokenResponse CreateToken(Dictionary<string, string> keyValuePairs);

        /// <summary>
        /// 验证身份 验证签名的有效性
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad">自定义各类验证； 是否包含那种申明，或者申明的值，例如：payLoad["aud"]?.ToString() == "AXJ"; </param>
        TokenResponseEnum Validate(string encodeJwt, Func<Dictionary<string, string>, bool> validatePayLoad, Action<Dictionary<string, string>> action);
    }

    /// <summary>
    /// jwt返回类
    /// </summary>
    public class JwtTokenResponse
    {
        /// <summary>
        /// token
        /// </summary>
        public string TokenStr { get; set; }
        /// <summary>
        /// 过期时间(秒)
        /// </summary>
        public DateTime Expires { get; set; }
    }
}
