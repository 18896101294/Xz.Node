using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Xz.Node.App.Interface;
using Xz.Node.App.SysLogs;
using Xz.Node.Framework.Common;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// 身份认证拦截
    /// </summary>
    public class OpenAuthFilter : IActionFilter
    {
        private readonly IAuth _authUtil;
        private readonly SysLogApp _logApp;

        /// <summary>
        /// 身份认证拦截
        /// </summary>
        /// <param name="authUtil"></param>
        /// <param name="logApp"></param>
        public OpenAuthFilter(IAuth authUtil, SysLogApp logApp)
        {
            _authUtil = authUtil;
            _logApp = logApp;
        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var description =
                (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor;

            var Controllername = description.ControllerName.ToLower();
            var Actionname = description.ActionName.ToLower();
            //匿名标识
            var authorize = description.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute));
            if (authorize != null)
            {
                return;
            }

            if (!_authUtil.CheckLogin())
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult(new Response
                {
                    Code = 401,
                    Message = "认证失败，请提供认证信息"
                });
                return;
            }
            _logApp.Add(new System_SysLogInfo
            {
                Content = $"用户访问",
                Href = $"{Controllername}/{Actionname}",
                CreateName = _authUtil.GetUserName(),
                CreateId = _authUtil.GetCurrentUser().User.Id,
                TypeName = "访问日志"
            });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }
    }
}
