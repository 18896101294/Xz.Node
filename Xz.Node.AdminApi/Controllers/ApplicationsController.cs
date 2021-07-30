using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xz.Node.App.AppManagers;
using Xz.Node.App.AppManagers.Request;
using Xz.Node.App.Base;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Controllers
{
    /// <summary>
    /// 应用管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "应用管理_Applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppManager _app;
        public ApplicationsController(AppManager app)
        {
            _app = app;
        }

        /// <summary>
        /// 加载应用列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<TableData> Load([FromQuery] QueryAppListReq request)
        {
            var applications = await _app.GetList(request);
            return new TableData
            {
                data = applications,
                count = applications.Count
            };
        }

        ///// <summary>
        ///// 获取分页数据
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult GetPageData(BaseDto.PageDataModel dto)
        //{
        //    var result = new ResultInfo<PageInfo<System_ApplicationInfo>>()
        //    {
        //        Message = "获取数据成功",
        //    };

        //    result.Data = _app.GetPageDatas(dto.Conditions.ToConditions(), dto.Sorts.ToSorts(), dto.PageIndex ?? 1, dto.PageSize ?? 20);

        //    return Ok(result);
        //}
    }
}