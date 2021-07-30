using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Jwt
{
    /// <summary>
    /// jwt帮助类
    /// </summary>
    public class JwtTokenHelper : IJwtTokenHelper
    {
        private IOptions<AppSetting> _appConfiguration;
        public JwtTokenHelper(IOptions<AppSetting> appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        /// <summary>
        /// 根据一个对象通过反射提供负载生成token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public JwtTokenResponse CreateToken<T>(T user) where T : class
        {
            //携带的负载部分，类似一个键值对
            List<Claim> claims = new List<Claim>();
            //这里我们用反射把model数据提供给它
            foreach (var item in user.GetType().GetProperties())
            {
                object obj = item.GetValue(user);
                string value = "";
                if (obj != null)
                    value = obj.ToString();

                claims.Add(new Claim(item.Name, value));
            }
            //创建token
            return CreateToken(claims);
        }

        /// <summary>
        /// 根据键值对提供负载生成token
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public JwtTokenResponse CreateToken(Dictionary<string, string> keyValuePairs)
        {
            //携带的负载部分，类似一个键值对
            List<Claim> claims = new List<Claim>();
            //这里我们通过键值对把数据提供给它
            foreach (var item in keyValuePairs)
            {
                claims.Add(new Claim(item.Key, item.Value));
            }
            //创建token
            return CreateToken(claims);
        }

        /// <summary>
        /// 创建token
        /// 注意报错：[PII is hidden]'. Exceptions caught:  '[PII is hidden]'. token: '[PII is hidden]
        /// 是因为签名秘钥长度不能太短了否则报错
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private JwtTokenResponse CreateToken(List<Claim> claims)
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromDays(_appConfiguration.Value.Jwt.AccessTokenExpiresDay));
            var token = new JwtSecurityToken(
                issuer: _appConfiguration.Value.Jwt.Issuer,//Token发布者
                audience: _appConfiguration.Value.Jwt.Audience,//Token接受者
                claims: claims,//携带的负载
                notBefore: now,//当前时间token生成时间
                expires: expires,//过期时间
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Value.Jwt.Secret)), SecurityAlgorithms.HmacSha256));
            return new JwtTokenResponse { TokenStr = new JwtSecurityTokenHandler().WriteToken(token), Expires = expires };
        }

        /// <summary>
        /// 验证身份 验证签名的有效性
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad">自定义各类验证； 是否包含那种申明，或者申明的值，例如：payLoad["aud"]?.ToString() == "AXJ"; </param>
        public TokenResponseEnum Validate(string encodeJwt, Func<Dictionary<string, string>, bool> validatePayLoad, Action<Dictionary<string, string>> action)
        {
            if (encodeJwt.Contains("Bearer "))
            {
                encodeJwt = encodeJwt.Substring(7, encodeJwt.Length - 7);
            }
            var jwtArr = encodeJwt.Split('.');
            if (jwtArr.Length < 3)//数据格式都不对直接pass
            {
                return TokenResponseEnum.Fail;
            }
            var header = JsonHelper.Instance.Deserialize<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[0]));
            var payLoad = JsonHelper.Instance.Deserialize<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[1]));
            //配置文件中取出来的签名秘钥
            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(_appConfiguration.Value.Jwt.Secret));
            //验证签名是否正确（把用户传递的签名部分取出来和服务器生成的签名匹配即可）
            var checkSuccess = string.Equals(jwtArr[2], Base64UrlEncoder.Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1])))));
            if (!checkSuccess)
            {
                return TokenResponseEnum.Fail;//签名不正确直接返回
            }

            //其次验证是否在有效期内（也应该必须）
            var now = ToUnixEpochDate(DateTime.UtcNow);
            if (!(now >= long.Parse(payLoad["nbf"].ToString()) && now < long.Parse(payLoad["exp"].ToString())))
            {
                return TokenResponseEnum.Expired;//token过期
            }
            //不需要自定义验证不传或者传递null即可
            if (validatePayLoad == null)
            {
                if (action != null)
                {
                    action(payLoad);
                }
                return TokenResponseEnum.Ok;
            }
            //再其次 进行自定义的验证
            if (!validatePayLoad(payLoad))
            {
                return TokenResponseEnum.Fail;
            }
            if (action != null)
            {
                action(payLoad);
            }
            return TokenResponseEnum.Ok;
        }

        public static long ToUnixEpochDate(DateTime date) =>
           (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}