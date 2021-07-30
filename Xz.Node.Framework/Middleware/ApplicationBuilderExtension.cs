using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.Framework.Middleware
{
    /// <summary>
    /// 日志中间件
    /// </summary>
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// 注入日志中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            //return builder.UseMiddleware<RequestResponseLoggingMiddleware>(); //此中间件会和httpreposts中间件冲突，导致post请求接口500，目前具体问题还没有查到

            return builder.UseMiddleware<RequRespLogMildd>();
        }
    }
}
