using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.Revelance.Request;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;
using System.Linq;

namespace Xz.Node.App.Auth.Module
{
    /// <summary>
    /// 模块管理
    /// </summary>
    public class ModuleApp : BaseTreeApp<Auth_ModuleInfo, XzDbContext>
    {
        private readonly RevelanceApp _app;
        /// <summary>
        /// 模块管理
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="app"></param>
        /// <param name="auth"></param>
        public ModuleApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_ModuleInfo, XzDbContext> repository
            , RevelanceApp app, IAuth auth) : base(unitWork, repository, auth)
        {
            _app = app;
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="model"></param>
        public void Add(Auth_ModuleInfo model)
        {
            var loginContext = _auth.GetCurrentUser();
            if (loginContext == null)
            {
                throw new InfoException("登录已过期", Define.INVALID_TOKEN);
            }

            CaculateCascade(model);

            Repository.Add(model);

            AddDefaultMenus(model);
            //当前登录用户的所有角色自动分配模块
            loginContext.Roles.ForEach(u =>
            {
                _app.Assign(new AssignReq
                {
                    type = Define.ROLEMODULE,
                    firstId = u.Id,
                    secIds = new[] { model.Id }
                });
            });
        }

        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="obj"></param>
        public void Update(Auth_ModuleInfo obj)
        {
            UpdateTreeObj(obj);
        }


        #region 用户/角色分配模块

        /// <summary>
        /// 加载特定角色的模块
        /// </summary>
        /// <param name="roleId">The role unique identifier.</param>
        public IEnumerable<Auth_ModuleInfo> LoadForRole(string roleId)
        {
            var moduleIds = UnitWork.Find<Auth_RelevanceInfo>(u => u.FirstId == roleId && u.Key == Define.ROLEMODULE)
                .Select(u => u.SecondId);
            return UnitWork.Find<Auth_ModuleInfo>(u => moduleIds.Contains(u.Id)).OrderBy(u => u.SortNo);
        }

        /// <summary>
        /// 获取角色可访问的模块字段
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public IEnumerable<string> LoadPropertiesForRole(string roleId, string moduleCode)
        {
            return _app.Get(Define.ROLEDATAPROPERTY, roleId, moduleCode);
        }

        /// <summary>
        /// 根据某角色ID获取可访问某模块的菜单项
        /// </summary>
        public IEnumerable<Auth_ModuleElementInfo> LoadMenusForRole(string moduleId, string roleId)
        {
            var elementIds = _app.Get(Define.ROLEELEMENT, true, roleId);
            var query = UnitWork.Find<Auth_ModuleElementInfo>(u => elementIds.Contains(u.Id));
            if (!string.IsNullOrEmpty(moduleId))
            {
                query = query.Where(u => u.ModuleId == moduleId);
            }
            return query;
        }

        #endregion 用户/角色分配模块


        #region 菜单操作

        /// <summary>
        /// 删除指定的菜单
        /// </summary>
        /// <param name="ids"></param>
        public void DelMenu(string[] ids)
        {
            UnitWork.Delete<Auth_ModuleElementInfo>(u => ids.Contains(u.Id));
            UnitWork.Save();
        }


        /// <summary>
        /// 新增菜单
        /// <para>当前登录用户的所有角色会自动分配菜单</para>
        /// </summary>
        public void AddMenu(Auth_ModuleElementInfo model)
        {
            var loginContext = _auth.GetCurrentUser();
            if (loginContext == null)
            {
                throw new InfoException("登录已过期", Define.INVALID_TOKEN);
            }

            UnitWork.ExecuteWithTransaction(() =>
            {
                UnitWork.Add(model);

                //当前登录用户的所有角色自动分配菜单
                loginContext.Roles.ForEach(u =>
                {
                    _app.Assign(new AssignReq
                    {
                        type = Define.ROLEELEMENT,
                        firstId = u.Id,
                        secIds = new[] { model.Id }
                    });
                });
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="model"></param>
        public void UpdateMenu(Auth_ModuleElementInfo model)
        {
            UnitWork.Update<Auth_ModuleElementInfo>(model);
            UnitWork.Save();
        }

        /// <summary>
        /// 添加默认按钮
        /// </summary>
        /// <param name="module"></param>
        private void AddDefaultMenus(Auth_ModuleInfo module)
        {
            AddMenu(new Auth_ModuleElementInfo
            {
                ModuleId = module.Id,
                DomId = "btnAdd",
                Script = "add()",
                Name = "添加",
                Sort = 1,
                Icon = "xinzeng",
                Class = "success",
                Remark = "新增" + module.Name
            });
            AddMenu(new Auth_ModuleElementInfo
            {
                ModuleId = module.Id,
                DomId = "btnEdit",
                Script = "edit()",
                Name = "编辑",
                Sort = 2,
                Icon = "bianji-copy",
                Class = "primary",
                Remark = "修改" + module.Name
            });
            AddMenu(new Auth_ModuleElementInfo
            {
                ModuleId = module.Id,
                DomId = "btnDel",
                Script = "del()",
                Name = "删除",
                Sort = 3,
                Icon = "shanchu",
                Class = "danger",
                Remark = "删除" + module.Name
            });

            //todo:可以自己添加更多默认按钮
        }

        #endregion
    }
}
