using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Xz.Node.App.Auth.Role;
using Xz.Node.App.Auth.Role.Request;
using Xz.Node.App.Auth.Role.Response;
using Xz.Node.App.Base;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "角色管理")]
    public class RoleController : Controller
    {
        private readonly RoleApp _app;
        /// <summary>
        /// 角色管理
        /// </summary>
        /// <param name="app"></param>
        public RoleController(RoleApp app)
        {
            _app = app;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LoadRolesPage([FromBody] LoadRolesPageReq req)
        {
            var result = new ResultInfo<PageInfo<LoadRolesPageView>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.LoadRolesPage(req);
            return Ok(result);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult AddRole([FromBody] Auth_RoleInfo req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "添加成功",
            };
            _app.AddRole(req);
            return Ok(result);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult UpdateRole([FromBody] Auth_RoleInfo req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "修改成功",
            };
            _app.UpdateRole(req);
            return Ok(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult Delete([FromBody] BaseIdReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功",
            };
            if (req.Ids == null || req.Ids.Count() == 0)
            {
                throw new Exception("请选择需要删除的用户");
            }
            _app.Delete(req.Ids.ToArray());
            return Ok(result);
        }
    }
}
