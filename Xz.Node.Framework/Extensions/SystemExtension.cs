using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Xz.Node.Framework.Extensions
{
    /// <summary>
    /// 系统信息扩展
    /// </summary>
    public static class SystemExtension
    {
        /// <summary>
        /// 获取客户Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientUserIp(this HttpContext context)
        {
            var userHostAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Connection.RemoteIpAddress.ToString();
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}
