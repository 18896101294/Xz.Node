using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.System.Configuration;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.AdminApi.Controllers.System.Configuration
{
    /// <summary>
    /// 系统配置管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "系统配管理")]
    public class SystemConfigurationController : ControllerBase
    {
        private readonly SystemConfigurationApp _app;
        /// <summary>
        /// 系统配置管理
        /// </summary>
        /// <param name="app"></param>
        public SystemConfigurationController(SystemConfigurationApp app)
        {
            _app = app;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(System_ConfigurationInfo data)
        {
            var result = new ResultInfo<System_ConfigurationInfo>()
            {
                Message = "保存成功",
                Data = data
            };

            if (data.KeyIsNull())
            {
                _app.Insert(data);
            }
            else
            {
                _app.Update(data);
            }
            return Ok(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(IList<Guid> ids)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功",
            };
            if (ids.Count() == 0)
            {
                throw new InfoException("删除Id不能为空");
            }
            _app.DeleteData(ids);
            return Ok(result);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageData([FromBody] BaseDto.PageDataModel dto)
        {
            var result = new ResultInfo<PageInfo<System_ConfigurationInfo>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.GetPageData(dto);
            return Ok(result);
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            var result = new ResultInfo<IList<string>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.GetAllCategory();
            return Ok(result);
        }

        /// <summary>
        /// 根据参数类型获取参数值
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSysConfigurations(string category)
        {
            var result = new ResultInfo<IList<System_ConfigurationInfo>>()
            {
                Message = "获取成功"
            };
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new InfoException("类型不能为空");
            }
            result.Data = _app.GetSysConfigurations(category);
            return Ok(result);
        }
    }
}
