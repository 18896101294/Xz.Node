using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// swagger 操作过滤器
    /// </summary>
    public class GlobalHttpHeaderOperationFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// swagger 操作过滤器
        /// </summary>
        /// <param name="configuration">配置中心</param>
        public GlobalHttpHeaderOperationFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 实现接口
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            #region 添加httpHeader参数
            bool isEnabledOAuth2 = _configuration.GetSection("AppSetting:OAuth2:Enabled").Value.ToBool();

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
                        Description = "token",
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
            #endregion

            #region 文件上传处理
            if (!context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                !context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileParameters = context.ApiDescription.ActionDescriptor.Parameters.Where(n => n.ParameterType == typeof(IFormFile)).ToList();
            var attrParameters = context.ApiDescription.ActionDescriptor.Parameters.Where(n => n.ParameterType.GetCustomAttribute<SwaggerFileUploadAttribute>() != null).ToList();

            var isPass = true;
            MethodInfo methodInfo;
            if (context.ApiDescription.TryGetMethodInfo(out methodInfo)
                && methodInfo.GetCustomAttribute<SwaggerFileUploadAttribute>() != null)
            {
                isPass = false;
            }

            if (fileParameters.Count < 0 && attrParameters.Count < 0 && isPass)
            {
                return;
            }

            if (fileParameters.Count > 0)
            {
                foreach (var fileParameter in fileParameters)
                {
                    var parameter = operation.Parameters.FirstOrDefault(n => n.Name == fileParameter.Name);
                    if (parameter != null)
                    {
                        operation.Parameters.Remove(parameter);
                        operation.RequestBody = GetRequestBody(parameter.Name);
                    }
                }
            }
            if (attrParameters.Count > 0)
            {
                foreach (var attrParameter in attrParameters)
                {
                    var parameter = operation.Parameters.FirstOrDefault(n => n.Name == attrParameter.Name);
                    if (parameter != null)
                    {
                        operation.Parameters.Remove(parameter);
                        operation.RequestBody = GetRequestBody(parameter.Name);
                    }
                }
            }
            if (isPass == false)
            {
                operation.RequestBody = GetRequestBody("formFile");
            }
            #endregion
        }

        private OpenApiRequestBody GetRequestBody(string fileName)
        {
            Dictionary<string, OpenApiMediaType> content = new Dictionary<string, OpenApiMediaType>();
            Dictionary<string, OpenApiSchema> fileProperties = new Dictionary<string, OpenApiSchema>();
            fileProperties.Add($"{fileName}", new OpenApiSchema
            {
                Type = "string",
                Format = "binary",
                AdditionalPropertiesAllowed = true,
                Nullable = true
            });
            var schem = new OpenApiSchema
            {
                Type = "object",
                AdditionalPropertiesAllowed = true,
                //Format = "file",
                Properties = fileProperties
            };
            content.Add("multipart/form-data", new OpenApiMediaType
            {
                Schema = schem,
                Encoding = schem.Properties.ToDictionary(entry => entry.Key, entry => new OpenApiEncoding 
                { 
                    Style = ParameterStyle.Form 
                })
            });
            var body = new OpenApiRequestBody();
            body.Content = content;
            return body;
        }
    }

    /// <summary>
    /// swagger 文档过滤器
    /// </summary>
    public class HiddenDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// 实现接口
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //过滤掉被标记为 HiddenAttribute 特性的接口
            foreach (ApiDescription apiDescription in context.ApiDescriptions)
            {
                var isPass = true;
                MethodInfo methodInfo;
                if (apiDescription.TryGetMethodInfo(out methodInfo)
                    && methodInfo.GetCustomAttribute<SwaggerHiddenAttribute>() != null)
                {
                    isPass = false;
                }

                if (isPass)
                {
                    if (apiDescription.CustomAttributes().OfType<SwaggerHiddenAttribute>().Count() > 0)
                    {
                        isPass = false;
                    }
                }

                if (!isPass)
                {
                    var key = "/" + apiDescription.RelativePath.TrimEnd('/');
                    //如果是测试模块则不用过滤
                    if (!key.ToLower().Contains("/test/") && swaggerDoc.Paths.ContainsKey(key))
                    {
                        swaggerDoc.Paths.Remove(key);
                    }
                }
            }
        }
    }
}
