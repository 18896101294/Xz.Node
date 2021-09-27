using Microsoft.AspNetCore.Mvc;
using Xz.Node.App.Base;
using Xz.Node.App.System.Notice;
using Xz.Node.App.System.Notice.Request;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Controllers.System.Notice
{
    /// <summary>
    /// 系统通知管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "通知管理")]
    public class SystemNoticeController : ControllerBase
    {
        private readonly SystemNoticeApp _app;
        /// <summary>
        /// 系统日志管理
        /// </summary>
        /// <param name="app"></param>
        public SystemNoticeController(SystemNoticeApp app)
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
            var result = new ResultInfo<PageInfo<System_NoticeInfo>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.GetPageData(dto);
            return Ok(result);
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete([FromBody] BaseIdsReq req)
        {
            var result = new ResultInfo<bool>()
            {
                Message = "删除成功"
            };

            _app.DeleteData(req);

            return Ok(result);
        }

        /// <summary>
        /// 重新执行
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult ReExecute([FromBody] ReExecuteReq req)
        {
            var result = new ResultInfo<bool>()
            {
                Message = "操作成功"
            };

            _app.ReExecute(req);

            return Ok(result);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult Add([FromBody] SaveNoticeReq req)
        {
            var result = new ResultInfo<bool>()
            {
                Message = "添加成功"
            };

            _app.Insert(new System_NoticeInfo()
            {
                Titile = req.Titile,
                Content = req.Content,
                Type = req.Type,
                ExecType = req.ExecType,
                RangeType = req.RangeType,
                ExecTime = req.ExecTime,
                RangeIds = string.Join(',', req.RangeIds),
                IsHtml = req.IsHtml,
                Status = req.Status,
                TenantId = req.TenantId
            });

            return Ok(result);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult Update([FromBody] SaveNoticeReq req)
        {
            var result = new ResultInfo<bool>()
            {
                Message = "修改成功"
            };

            _app.Update(req);

            return Ok(result);
        }
    }
}
