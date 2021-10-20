using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Xz.Node.Framework.Extensions
{
    /// <summary>
    /// 系统信息扩展
    /// </summary>
    public static class SystemExtension
    {
        /// <summary>
        /// 获取客户端Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientUserIp(this HttpContext context)
        {
            var userHostAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
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


        /// <summary>  
        /// 是否能 Ping 通指定的主机  
        /// </summary>  
        /// <param name="ip">ip 地址或主机名或域名</param>  
        /// <returns>true 通，false 不通</returns>  
        public static bool Ping(string ip)
        {
            using var p = new Ping();
            var options = new PingOptions();
            options.DontFragment = true;
            var data = "Test Data!";
            var buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒  
            var reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
