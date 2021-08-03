using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// swagger请求的时候，如果是Identity方式，自动加授权方式
    /// </summary>
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="appConfiguration"></param>
        public AuthResponsesOperationFilter()
        {
        }

        /// <summary>
        /// 添加权限小锁
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var configuration = ConfigHelper.GetConfigRoot();
            bool isEnabledOAuth2 = configuration["AppSetting:OAuth2:Enabled"].ToBool();

            if (isEnabledOAuth2)
            {
                return;
            }

            var anonymous = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AllowAnonymousAttribute>().Any();
            if (!anonymous)
            {
                var security = new List<OpenApiSecurityRequirement>();
                security.Add(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] { "xznodeapi" }
                    }
                });
                operation.Security = security;
                //operation.Security = new List<OpenApiSecurityRequirement>
                //{
                //  new Dictionary<string, IEnumerable<string>> {{"oauth2", new[] { "openauthapi" } }}
                //};
            }
        }
    }
}
