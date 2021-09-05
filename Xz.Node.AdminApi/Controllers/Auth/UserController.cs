using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.Auth.User;
using Xz.Node.App.Auth.User.Request;
using Xz.Node.App.Auth.User.Response;
using Xz.Node.App.Base;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "用户管理")]
    public class UserController : Controller
    {
        private readonly UserApp _app;
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="app"></param>
        public UserController(UserApp app)
        {
            _app = app;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LoadUsersPage([FromBody] LoadUsersPageReq req)
        {
            var result = new ResultInfo<PageInfo<LoadUsersPageView>>()
            {
                Message = "获取成功"
            };
            result.Data = _app.LoadUsersPage(req);
            return Ok(result);
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult SaveUser([FromBody] UpdateUserReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "保存成功",
            };
             _app.SaveUser(req);
            return Ok(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult Delete([FromBody] DeleteUserReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功",
            };
            if(req.Ids == null || req.Ids.Count() == 0)
            {
                throw new Exception("请选择需要删除的用户");
            }
            _app.Delete(req.Ids.ToArray());
            return Ok(result);
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DisableUser([FromBody] BaseIdsReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "禁用成功",
            };
            _app.DisableUser(req);
            return Ok(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = $"重置成功，初始密码为：{Define.INITIAL_PWD}",
            };
            if (string.IsNullOrEmpty(req.Id))
            { 
                throw new Exception("请选择需要重置密码的用户");
            }
            _app.ChangePassword(req);
            return Ok(result);
        }

        /// <summary>
        /// 修改基本资料
        /// </summary>
        /// <param name="req"></param>
        [HttpPost]
        public IActionResult ChangeProfile([FromBody] ChangeProfileReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "保存成功",
                Data = req
            };
            if (string.IsNullOrEmpty(req.Id))
            {
                throw new Exception("用户id不能为空");
            }
            _app.ChangeProfile(req);
            return Ok(result);
        }
    }
}
