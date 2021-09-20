using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Xz.Node.Interactive.Controllers
{
    /// <summary>
    /// Consul接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Consul接口")]
    public class ConsulController : ControllerBase
    {
        /// <summary>
        /// 系统接口
        /// </summary>
        /// <param name="configuration"></param>
        public ConsulController()
        {
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
    }
}
