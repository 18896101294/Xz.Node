using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Revelance.Request;
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
        /// 获取角色绑定的用户列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetRoleBindUsers([FromBody] BaseIdReq req)
        {
            var result = new ResultInfo<List<RoleBindUsersView>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.GetRoleBindUsers(req);
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
        /// 为角色分配用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RoleAllocationUsers([FromBody] AssignRoleUsersReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "分配成功",
            };
            _app.RoleAllocationUsers(req);
            return Ok(result);
        }

        /// <summary>
        /// 为角色分配模块
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RoleAllocationModules([FromBody] AssignRoleModulesReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "分配成功",
            };
            _app.RoleAllocationModules(req);
            return Ok(result);
        }

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RoleAllocationMenus([FromBody] List<AssignRoleMenusReq> reqs)
        {
            var result = new ResultInfo<object>()
            {
                Message = "分配成功",
            };
            _app.RoleAllocationMenus(reqs);
            return Ok(result);
        }

        /// <summary>
        /// 为角色分配字段
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RoleAllocationDatas([FromBody] List<AssignDataReq> reqs)
        {
            var result = new ResultInfo<object>()
            {
                Message = "分配成功",
            };
            _app.RoleAllocationDatas(reqs);
            return Ok(result);
        }

        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DisableRole([FromBody] BaseIdsReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "禁用成功",
            };
            _app.DisableRole(req);
            return Ok(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult Delete([FromBody] BaseIdsReq req)
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
