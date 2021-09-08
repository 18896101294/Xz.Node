using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Module;
using Xz.Node.App.Auth.Module.Request;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 模块管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "模块管理")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleApp _app;
        private readonly IAuth _authUtil;
        /// <summary>
        /// 系统配置管理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="authUtil"></param>
        public ModuleController(ModuleApp app, IAuth authUtil)
        {
            _app = app;
            _authUtil = authUtil;
        }

        /// <summary>
        /// 获取勾选模块的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetCheckedModules([FromBody] BaseIdsReq req)
        {

        }

        /// <summary>
        /// 加载角色模块
        /// </summary>
        [HttpGet]
        public IActionResult LoadForRole([FromQuery] string firstId)
        {
            var result = new ResultInfo<IList<string>>()
            {
                Message = "获取成功",
            };
            result.Data = _app.LoadForRole(firstId).Select(o => o.Id).ToList();
            return Ok(result);
        }
        /// <summary>
        /// 获取角色已经分配的字段
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="moduleCode">模块代码，如Category</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LoadPropertiesForRole(string roleId, string moduleCode)
        {
            var result = new ResultInfo<IList<string>>()
            {
                Message = "获取成功",
            };
            result.Data = _app.LoadPropertiesForRole(roleId, moduleCode).ToList();
            return Ok(result);
        }

        /// <summary>
        /// 根据某角色ID获取可访问某模块的菜单项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LoadMenusForRole(string moduleId, string firstId)
        {
            var result = new ResultInfo<IList<Auth_ModuleElementInfo>>()
            {
                Message = "获取成功",
            };
            result.Data = _app.LoadMenusForRole(moduleId, firstId).ToList();
            return Ok(result);
        }

        /// <summary>
        /// 获取发起页面的菜单权限
        /// </summary>
        /// <returns>System.String.</returns>
        [HttpGet]
        public IActionResult LoadAuthorizedMenus(string modulecode)
        {
            var result = new ResultInfo<ModuleView>()
            {
                Message = "获取成功",
            };
            var user = _authUtil.GetCurrentUser();
            result.Data = user.Modules.First(u => u.Code == modulecode);
            return Ok(result);
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] Auth_ModuleInfo model)
        {
            var result = new ResultInfo<Auth_ModuleInfo>()
            {
                Message = "保存成功",
                Data = model
            };
            _app.Add(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] Auth_ModuleInfo model)
        {
            var result = new ResultInfo<object>()
            {
                Message = "保存成功",
            };
            _app.Update(model);
            return Ok(result);
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete([FromBody] ModuleDeleteReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功",
            };
            _app.Delete(req);
            return Ok(result);
        }

        /// <summary>
        /// 加载当前用户可访问模块的菜单
        /// </summary>
        /// <param name="req">模块入参</param>
        [HttpGet]
        public IActionResult LoadMenus([FromQuery] LoadMenusReq req)
        {
            var result = new ResultInfo<List<Auth_ModuleElementInfo>>()
            {
                Message = "获取成功",
            };
            if(string.IsNullOrEmpty(req.ModuleId))
            {
                throw new InfoException("模块Id不能为空");
            }
            var user = _authUtil.GetCurrentUser();
            var module = user.Modules.Single(u => u.Id == req.ModuleId);
            result.Data = module.Elements;
            return Ok(result);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddMenu([FromBody] Auth_ModuleElementInfo model)
        {
            var result = new ResultInfo<Auth_ModuleElementInfo>()
            {
                Message = "保存成功",
                Data = model
            };
            _app.AddMenu(model);
            return Ok(result);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateMenu([FromBody] Auth_ModuleElementInfo model)
        {
            var result = new ResultInfo<object>()
            {
                Message = "保存成功"
            };
            _app.UpdateMenu(model);
            return Ok(result);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpPost]
        public IActionResult DelMenu([FromBody] DelMenuReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功"
            };
            _app.DelMenu(req);

            return Ok(result);
        }
    }
}
