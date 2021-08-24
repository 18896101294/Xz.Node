using Microsoft.AspNetCore.Mvc;
using Xz.Node.App.Request;
using Xz.Node.App.SysLogs;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Controllers.SysLogs
{
    /// <summary>
    /// 系统日志管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "系统日志管理")]
    public class SysLogController : Controller
    {
        private readonly SysLogApp _app;
        /// <summary>
        /// 系统日志管理
        /// </summary>
        /// <param name="app"></param>
        public SysLogController(SysLogApp app)
        {
            _app = app;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageData([FromBody] BaseDto.PageDataModel dto)
        {
            var result = new ResultInfo<PageInfo<System_SysLogInfo>>()
            {
                Message = "获取成功"
            };
            //result.Data = _app.Load(dto);
            result.Data = _app.GetPageData(dto);
            return Ok(result);
        }

        /// <summary>
        /// 清理10天前的日志数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ClearData()
        {
            var result = new ResultInfo<object>()
            {
                Message = "清理成功"
            };
            _app.ClearData();
            return Ok(result);
        }
    }
}
