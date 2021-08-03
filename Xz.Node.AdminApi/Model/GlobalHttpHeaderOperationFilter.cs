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
    ///  添加httpHeader参数
    /// </summary>
    public class GlobalHttpHeaderOperationFilter : IOperationFilter
    {
        /// <summary>
        ///  添加httpHeader参数
        /// </summary>
        /// <param name="appConfiguration"></param>
        public GlobalHttpHeaderOperationFilter()
        {
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var configuration = ConfigHelper.GetConfigRoot();
            bool isEnabledOAuth2 = configuration["AppSetting:OAuth2:Enabled"].ToBool();

            //如果是Identity认证方式，不需要界面添加x-token得输入框
            if (isEnabledOAuth2)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                var actionAttrs = context.ApiDescription.ActionDescriptor.EndpointMetadata;
                var isAnony = actionAttrs.Any(a => a.GetType() == typeof(AllowAnonymousAttribute));

                //不是匿名，则添加默认的X-Token
                if (!isAnony)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = Define.TOKEN_NAME,
                        In = ParameterLocation.Header,
                        Description = "当前登录用户登录token",
                        Required = false
                    });
                }

                //var headers = new Dictionary<string, string> { { "appKey", "分配的AppKey" } };
                //foreach (var item in headers)
                //{
                //    operation.Parameters.Add(new OpenApiParameter()
                //    {
                //        Name = item.Key,
                //        In = ParameterLocation.Header,//query header body path formData
                //        Schema = new OpenApiSchema { Type = "string" },
                //        Description = item.Value,
                //        Required = true //是否必选
                //    });
                //}
            }
        }
    }
}
