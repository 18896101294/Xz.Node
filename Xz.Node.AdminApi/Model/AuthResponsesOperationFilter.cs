using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// swagger请求的时候，如果是Identity方式，自动加授权方式
    /// </summary>
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// swagger请求的时候，如果是Identity方式，自动加授权方式
        /// </summary>
        /// <param name="configuration"></param>
        public AuthResponsesOperationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 添加权限小锁
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool isEnabledOAuth2 = _configuration.GetSection("AppSetting:OAuth2:Enabled").Value.ToBool();

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
