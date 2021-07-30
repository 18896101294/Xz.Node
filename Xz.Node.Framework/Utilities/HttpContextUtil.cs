using Microsoft.AspNetCore.Http;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions.AutofacManager;

namespace Xz.Node.Framework.Utilities
{
    /// <summary>
    /// HttpContext Util
    /// </summary>
    public static class HttpContextUtil
    {
        private static IHttpContextAccessor _accessor = AutofacContainerModule.GetService<IHttpContextAccessor>();
        public static HttpContext Current => _accessor.HttpContext;

        /// <summary>
        /// 获取租户ID
        /// </summary>
        /// <returns></returns>
        public static string GetTenantId(this IHttpContextAccessor accessor)
        {
            string tenantId = "XzNodeDBContext";//默认租户
            if (accessor != null && accessor.HttpContext != null)
            {
                //读取多租户ID
                var httpTenantId = accessor.HttpContext.Request.Query[Define.TENANT_ID];
                if (string.IsNullOrEmpty(httpTenantId))
                {
                    httpTenantId = accessor.HttpContext.Request.Headers[Define.TENANT_ID];
                }

                //如果没有租户id，或租户用的是默认的OpenAuthDBContext,则不做任何调整
                if (!string.IsNullOrEmpty(httpTenantId))
                {
                    tenantId = httpTenantId;
                }
            }
            return tenantId;
        }
    }
}
