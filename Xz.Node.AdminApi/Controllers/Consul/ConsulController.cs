﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers.Consul
{
    /// <summary>
    /// Consul管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Consul管理接口")]
    public class ConsulController : ControllerBase
    {
        private readonly ConsulConfig _consulConfig;
        private readonly HttpHelper _httpHelper = null;
        /// <summary>
        /// Consul管理
        /// </summary>
        /// <param name="options"></param>
        public ConsulController(IOptions<ConsulConfig> options)
        {
            _consulConfig = options.Value;
            _httpHelper = new HttpHelper(_consulConfig.ConsulAddress);
        }

        /// <summary>
        /// 获取Consul服务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConsulServices()
        {
            var result = new ResultInfo<string>()
            {
                Message = "获取成功"
            };

            var resultData = _httpHelper.Get(null, "/v1/catalog/services");

            result.Data = resultData;

            return Ok(result);
        }
    }
}
