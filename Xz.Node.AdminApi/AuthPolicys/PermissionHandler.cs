using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Xz.Node.AdminApi.AuthPolicys
{
    /// <summary>
    /// 权限校验
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="accessor"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            Schemes = schemes;
        }

        /// <summary>
        /// 重写异步处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            context.Succeed(requirement);
            return;
            var httpContext = _accessor.HttpContext;

            //是否不校验权限
            bool isUnCheckPermission = false;
            //获取特性
            Endpoint endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint != null)
            {
                var unCheckPermissionAttribute = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
                if (unCheckPermissionAttribute != null)
                {
                    isUnCheckPermission = true;
                }
            }

            //请求Url
            if (httpContext != null)
            {
                var questUrl = httpContext.Request.Path.Value.ToLower();
                //判断请求是否停止
                var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(httpContext, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        context.Fail();
                        return;
                    }
                }
                //判断请求是否拥有凭据，即有没有登录
                var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                    //result?.Principal不为空即登录成功
                    if (result?.Principal != null)
                    {
                        httpContext.User = result.Principal;
                        // 校验是不是AdminApi颁发的token
                        string apitype = httpContext.User.Claims.SingleOrDefault(s => s.Type == "apitype")?.Value;
                        if (string.IsNullOrEmpty(apitype))
                        {
                            context.Fail();
                            return;
                        }

                        if (apitype != "api".ToString())
                        {
                            context.Fail();
                            return;
                        }
                        // 获取当前用户的角色信息
                        var currentUserRoles = new List<string>();
                        // ids4和jwt切换
                        // ids4
                        
                            // jwt
                        currentUserRoles = (from item in httpContext.User.Claims
                                            where item.Type == "1"
                                            select item.Value).ToList();

                        var isMatchRole = false;

                        if (isUnCheckPermission == true)
                        {
                            //不校验权限
                            isMatchRole = true;
                        }
                        else
                        {
                           
                        }

                        //验证权限
                        if (currentUserRoles.Count <= 0 || !isMatchRole)
                        {
                            context.Fail();
                            return;
                        }

                        var isExp = (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null && DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now;
                        //token有效期校验
                        if (!isExp)
                        {
                            context.Fail();
                            return;
                        }
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
