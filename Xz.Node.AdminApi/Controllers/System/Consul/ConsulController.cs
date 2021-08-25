using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xz.Node.App.System.Consul.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers.System.Consul
{
    /// <summary>
    /// Consul管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Consul管理")]
    public class ConsulController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpHelper _httpHelper = null;
        /// <summary>
        /// Consul管理
        /// </summary>
        /// <param name="configuration"></param>
        public ConsulController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpHelper = new HttpHelper(_configuration["Consul:ConsulAddress"]);
            //_httpHelper = new HttpHelper("http://1.116.5.70:8600");
        }
        /// <summary>
        /// 把健康检查的地址简单实现一下
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        /// <summary>
        /// 获取Consul服务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConsulServices()
        {
            var result = new ResultInfo<List<string>>()
            {
                Message = "获取成功"
            };

            var resultData = new List<string>();

            var resultDataStr = _httpHelper.Get(null, "/v1/catalog/services");

            var resultDataDic = JsonHelper.Instance.JsonStringToKeyValuePairs(resultDataStr);

            if (resultDataDic.Count > 0)
            {
                foreach (var keyValuePair in resultDataDic)
                {
                    if (!keyValuePair.Key.Equals("consul"))
                    {
                        resultData.Add(keyValuePair.Key);
                    }
                }
            }
            result.Data = resultData;

            return Ok(result);
        }

        /// <summary>
        /// 获取Consul服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConsulServiceItem(string serviceName)
        {
            var result = new ResultInfo<List<ConsulServiceItemView>>()
            {
                Message = "获取成功"
            };

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new InfoException("Consul服务名称不能为空");
            }

            var resultDataStr = _httpHelper.Get(null, $"/v1/catalog/service/{serviceName}");

            var resultData = JsonHelper.Instance.Deserialize<List<ConsulServiceItemView>>(resultDataStr);

            result.Data = resultData;

            return Ok(result);
        }

        /// <summary>
        /// 获取Consul服务配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConsulSelf()
        {
            var result = new ResultInfo<ConsulSelfView>()
            {
                Message = "获取成功"
            };

            var resultDataStr = _httpHelper.Get(null, $"/v1/agent/self");

            var resultData = JsonHelper.Instance.Deserialize<ConsulSelfView>(resultDataStr);

            result.Data = resultData;

            return Ok(result);
        }

        /// <summary>
        /// 删除Consul服务实例
        /// </summary>
        /// <param name="serviceId">服务id</param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ConsulServiceDeleteItem(Guid serviceId)
        {
            var result = new ResultInfo<string>()
            {
                Message = "删除成功"
            };

            if (!serviceId.ToString().IsGuid())
            {
                throw new InfoException("Consul服务id必须大于0");
            }

            var resultDataStr = _httpHelper.Put(null, $"/v1/agent/service/deregister/{serviceId}");

            result.Data = resultDataStr;

            return Ok(result);
        }
    }
}
