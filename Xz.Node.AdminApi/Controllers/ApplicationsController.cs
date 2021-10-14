using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xz.Node.AdminApi.Model;
using Xz.Node.App.AppManagers;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Controllers
{
    /// <summary>
    /// 应用管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "应用管理")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppManager _app;
        private readonly IConfiguration _configuration;
        private readonly bool _isEnabledId4 = false;
        /// <summary>
        /// 应用管理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public ApplicationsController(AppManager app,
            IConfiguration configuration)
        {
            _app = app;
            _configuration = configuration;
            _isEnabledId4 = _configuration["AppSetting:IdentityServer4:Enabled"].ToBool();
        }

        /// <summary>
        /// 加载应用列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Load()
        {
            var result = new ResultInfo<List<System_ApplicationInfo>>()
            {
                Message = "获取数据成功",
            };
            var resultData = _app.GetList();
            result.Data = resultData;
            return Ok(result);
        }

        /// <summary>
        /// 获取系统是否启用了Id4的登录方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Hidden]
        public IActionResult EnabledId4LoginWay()
        {
            var result = new ResultInfo<bool>()
            {
                Message = "获取数据成功",
                Data = _isEnabledId4
            };
            return Ok(result);
        }
    }
}