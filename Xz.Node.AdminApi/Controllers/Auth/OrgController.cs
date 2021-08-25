using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.Auth.Org;
using Xz.Node.App.Auth.Org.Request;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 部门管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "部门管理")]
    public class OrgController : Controller
    {
        private readonly OrgApp _app;
        private readonly IAuth _authUtil;
        /// <summary>
        /// 部门管理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="authUtil"></param>
        public OrgController(OrgApp app, IAuth authUtil)
        {
            _app = app;
            _authUtil = authUtil;
        }

        /// <summary>
        /// 获取指定用户所能访问的部门
        /// </summary>
        /// <param name="req">用户入参</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LoadForUser([FromQuery] LoadForUserReq req)
        {
            var result = new ResultInfo<IList<Auth_OrgInfo>>()
            {
                Message = "获取成功",
            };
            if(string.IsNullOrEmpty(req.UserId))
            {
                throw new InfoException("用户id不能为不空");
            }
            var orgs = _app.LoadForUser(req.UserId);
            result.Data = orgs;
            return Ok(result);
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="org">部门数据</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddOrg([FromBody] Auth_OrgInfo org)
        {
            var result = new ResultInfo<Auth_OrgInfo>()
            {
                Message = "添加成功",
                Data = org
            };
            _app.Add(org);
            return Ok(result);
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="org">部门数据</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateOrg([FromBody] Auth_OrgInfo org)
        {
            var result = new ResultInfo<Auth_OrgInfo>()
            {
                Message = "修改成功",
                Data = org
            };
            _app.Update(org);
            return Ok(result);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="req">删除部门入参</param>
        [HttpPost]
        public IActionResult DeleteOrg([FromBody] OrgDeleteReq req)
        {
            if (req.Ids == null ||req.Ids.Count() == 0)
            {
                throw new InfoException("删除Id不能为空");
            }
            var result = new ResultInfo<Auth_OrgInfo>()
            {
                Message = "删除成功",
            };
            _app.DelOrgCascade(req.Ids);
            return Ok(result);
        }
    }
}
