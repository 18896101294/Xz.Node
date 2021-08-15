using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.Auth.Request;
using Xz.Node.App.Auth.Response;
using Xz.Node.App.AuthStrategies;
using Xz.Node.App.Interface;
using Xz.Node.App.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "用户管理")]
    public class UserController : ControllerBase
    {
        private readonly AuthStrategyContext _authStrategyContext;
        private readonly IAuth _authUtil;
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="authUtil"></param>
        public UserController(IAuth authUtil)
        {
            _authUtil = authUtil;
            _authStrategyContext = _authUtil.GetCurrentUser();
        }

        /// <summary>
        /// 获取用户登录名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserName()
        {
            var result = new ResultInfo<string>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.User.Name;
            return Ok(result);
        }

        /// <summary>
        /// 获取用户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserProfile()
        {
            var result = new ResultInfo<UserProfileView>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.User.MapTo<UserProfileView>();
            return Ok(result);
        }

        ///// <summary>
        ///// 获取登录用户可访问的所有模块，及模块的操作菜单
        ///// </summary>
        //[HttpGet]
        //public IActionResult GetModulesTree()
        //{
        //    var result = new ResultInfo<List<TreeItem<ModuleView>>>()
        //    {
        //        Message = "获取数据成功",
        //    };
        //    result.Data = _authStrategyContext.Modules.GenerateTree(u => u.Id, u => u.ParentId).ToList();
        //    return Ok(result);
        //}

        /// <summary>
        /// 获取登录用户可访问的所有模块，及模块的操作菜单
        /// </summary>
        [HttpGet]
        public IActionResult GetModulesTree([FromQuery] GetModuleReq req)
        {
            var result = new ResultInfo<List<ModuleView>>()
            {
                Message = "获取数据成功",
            };
            var modules = _authStrategyContext.Modules.Where(o=>o.ParentId == req.ParentId).ToList();
            foreach (var module in modules)
            {
                var childrenCount = _authStrategyContext.Modules.Count(o => o.ParentId == module.Id);
                module.HasChildren = childrenCount > 0 ? true : false;
            }
            result.Data = modules;
            return Ok(result);
        }

        /// <summary>
        /// 获取datatable结构的模块列表
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetModulesTable(string pId)
        {
            var result = new ResultInfo<List<ModuleView>>()
            {
                Message = "获取数据成功",
            };
            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(pId))
            {
                var obj = _authStrategyContext.Modules.SingleOrDefault(u => u.Id == pId);
                if (obj == null)
                    throw new InfoException("未能找到指定对象信息");
                cascadeId = obj.CascadeId;
            }

            var query = _authStrategyContext.Modules.Where(u => u.CascadeId.Contains(cascadeId));

            if (query == null || query.Count() == 0)
            {
                result.Message = "暂无数据";
                return Ok(result);
            }
            result.Data = query.ToList();

            return Ok(result);
        }

        /// <summary>
        /// 获取用户可访问的模块列表
        /// </summary>
        [HttpGet]
        public IActionResult GetModules()
        {
            var result = new ResultInfo<List<ModuleView>>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.Modules;
            return Ok(result);
        }

        /// <summary>
        /// 获取登录用户可访问的所有部门
        /// <para>用于树状结构</para>
        /// </summary>
        [HttpGet]
        public IActionResult GetOrgs()
        {
            var result = new ResultInfo<List<Auth_OrgInfo>>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.Orgs;
            return Ok(result);
        }

        /// <summary>
        /// 获取当前登录用户可访问的字段
        /// </summary>
        /// <param name="moduleCode">模块的Code，如Category</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProperties(string moduleCode)
        {
            var result = new ResultInfo<List<KeyDescription>>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.GetProperties(moduleCode);
            return Ok(result);
        }

        /// <summary>
        /// 加载机构的全部下级机构
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSubOrgs(string orgId)
        {
            var result = new ResultInfo<List<Auth_OrgInfo>>()
            {
                Message = "获取数据成功",
            };
            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(orgId))
            {
                var org = _authStrategyContext.Orgs.SingleOrDefault(u => u.Id == orgId);
                if (org == null)
                {
                    throw new InfoException($"Id为：{orgId}的组织不存在");
                }
                cascadeId = org.CascadeId;
            }
            var query = _authStrategyContext.Orgs.Where(u => u.CascadeId.Contains(cascadeId));

            result.Data = query.ToList();

            return Ok(result);
        }
    }
}