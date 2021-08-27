using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Module.Request;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.Auth.Org.Response;
using Xz.Node.App.AuthStrategies;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.AdminApi.Controllers.Auth
{
    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "登录信息")]
    public class UserSessionController : ControllerBase
    {
        private readonly AuthStrategyContext _authStrategyContext;
        private readonly IAuth _authUtil;
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="authUtil"></param>
        public UserSessionController(IAuth authUtil)
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
            result.Data.Roles = _authStrategyContext.Roles.Select(o => o.Code).ToList();
            return Ok(result);
        }

        /// <summary>
        /// 获取登录用户可访问的所有模块，及模块的操作菜单
        /// </summary>
        [HttpGet]
        public IActionResult GetUserModulesTree()
        {
            var result = new ResultInfo<List<TreeItem<ModuleView>>>()
            {
                Message = "获取数据成功",
            };
            result.Data = _authStrategyContext.Modules.GenerateTree(u => u.Id, u => u.ParentId).ToList();
            return Ok(result);
        }

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
            var modules = _authStrategyContext.Modules.Where(o => o.ParentId == req.ParentId).OrderBy(o => o.SortNo).ToList();
            foreach (var module in modules)
            {
                var childrenCount = _authStrategyContext.Modules.Count(o => o.ParentId == module.Id);
                module.HasChildren = childrenCount > 0 ? true : false;
            }
            result.Data = modules;
            return Ok(result);
        }

        /// <summary>
        /// 获取用户可访问模块的模块名称集合,用于下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetModulesName()
        {
            var result = new ResultInfo<List<ModulesNameView>>()
            {
                Message = "获取数据成功",
            };
            var modulesNameData = _authStrategyContext.Modules.OrderBy(o => o.SortNo).Select(o => new ModulesNameView() { Id = o.Id, Name = o.Name, ParentId = o.ParentId }).ToList();
            result.Data = modulesNameData;
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
            result.Data = _authStrategyContext.Orgs.OrderBy(o => o.SortNo).ToList();
            return Ok(result);
        }

        /// <summary>
        /// 获取用户可访问部门名称集合,用于下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrgsName()
        {
            var result = new ResultInfo<List<OrgsNameView>>()
            {
                Message = "获取数据成功",
            };
            var modulesNameData = _authStrategyContext.Orgs.OrderBy(o => o.SortNo).Select(o => new OrgsNameView() { Id = o.Id, Name = o.Name, ParentId = o.ParentId }).ToList();
            result.Data = modulesNameData;
            return Ok(result);
        }

        /// <summary>
        /// 获取指定部门的全部下级机构
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetChildOrgs(string orgId)
        {
            var result = new ResultInfo<List<Auth_OrgInfo>>()
            {
                Message = "获取数据成功",
            };
            if (string.IsNullOrEmpty(orgId))
            {
                throw new InfoException("部门id不能为空");
            }
            if(orgId == "0")
            {
                result.Data = _authStrategyContext.Orgs.OrderBy(o => o.SortNo).ToList();
                return Ok(result);
            }
            var query = _authStrategyContext.Orgs.Where(u => u.ParentId == orgId).OrderBy(o => o.SortNo);
            if(query == null || query.Count() == 0)
            {
                result.Data = _authStrategyContext.Orgs.Where(u => u.Id == orgId).ToList();
                return Ok(result);
            }
            result.Data = query.ToList();
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
    }
}