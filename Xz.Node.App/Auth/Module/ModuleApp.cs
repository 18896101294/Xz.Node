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
using Xz.Node.App.Auth.Module.Request;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.System.Configuration;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.App.Auth.Module
{
    /// <summary>
    /// 模块管理
    /// </summary>
    public class ModuleApp : BaseTreeApp<Auth_ModuleInfo, XzDbContext>
    {
        private readonly RevelanceApp _app;
        private readonly SystemConfigurationApp _configurationApp;
        private readonly DbExtension _dbExtension;

        /// <summary>
        /// 模块管理
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="app"></param>
        /// <param name="auth"></param>
        /// <param name="configurationApp"></param>
        /// <param name="dbExtension"></param>
        public ModuleApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_ModuleInfo, XzDbContext> repository
            , RevelanceApp app, IAuth auth,
            SystemConfigurationApp configurationApp,
            DbExtension dbExtension) : base(unitWork, repository, auth)
        {
            _app = app;
            _configurationApp = configurationApp;
            _dbExtension = dbExtension;
        }

        /// <summary>
        /// 获取勾选模块的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public List<CheckedModulesView> GetCheckedModules(BaseIdsReq req)
        {
            var resultData = new List<CheckedModulesView>();

            var modules = UnitWork.Find<Auth_ModuleInfo>(null).ToList();
            var moduleelements = UnitWork.Find<Auth_ModuleElementInfo>(null).ToList();

            var moduleDatas = modules.Where(o => req.Ids.Contains(o.Id)).OrderBy(o => o.SortNo);

            foreach (var moduleData in moduleDatas)
            {
                if (!string.IsNullOrEmpty(moduleData.ParentId))
                {
                    resultData.Add(new CheckedModulesView()
                    {
                        Id = moduleData.Id,
                        Name = moduleData.Name,
                        FullName = this.SetFullName(moduleData.Name, moduleData, modules),
                        elements = moduleelements.Where(o => o.ModuleId == moduleData.Id).OrderBy(o => o.Sort).ToList()
                    });
                }
            }
            return resultData;
        }

        private string SetFullName(string name, Auth_ModuleInfo data, List<Auth_ModuleInfo> modules)
        {
            string fullName = name;
            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parentData = modules.FirstOrDefault(o => o.Id == data.ParentId);
                fullName = parentData.Name + "》" + fullName;
                this.SetFullName(fullName, parentData, modules);
            }
            return fullName;
        }

        /// <summary>
        /// 获取勾选的模块下的数据字典
        /// </summary>
        /// <param name="req"></param>
        public List<CheckedPropertiesView> GetCheckedProperties(BaseIdsReq req)
        {
            var resultData = new List<CheckedPropertiesView>();

            var modules = UnitWork.Find<Auth_ModuleInfo>(null).ToList();
            var moduleDatas = modules.Where(o => req.Ids.Contains(o.Id) && o.IsSys == false).OrderBy(o => o.SortNo);

            var moduleViewNames = moduleDatas.Select(o => o.Code).ToList();
            var dataPropertyConfigs = _configurationApp.GetSysConfigurations("SystemDataProperty").Where(o => moduleViewNames.Contains(o.Text));
           
            foreach (var moduleData in moduleDatas)
            {
                var dataPropertyConfig = dataPropertyConfigs.FirstOrDefault(o => o.Text == moduleData.Code);
                if (!string.IsNullOrEmpty(moduleData.ParentId) && dataPropertyConfig != null)
                {
                    resultData.Add(new CheckedPropertiesView()
                    {
                        Id = moduleData.Id,
                        Name = moduleData.Name,
                        FullName = this.SetFullName(moduleData.Name, moduleData, modules),
                        Keys = _dbExtension.GetKeyDescription(dataPropertyConfig.Value, moduleData.Id)
                    });
                }
            }
            return resultData;
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

            if (!string.IsNullOrEmpty(model.ParentId))
            {
                //不是根节点才添加菜单数据
                //AddDefaultMenus(model);
            }
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
            var oldObj = UnitWork.FirstOrDefault<Auth_ModuleInfo>(o => o.Id == obj.Id);
            if(oldObj.Code != obj.Code)
            {
                //同步更新模块字段配置
                var dataPropertyConfig = UnitWork.FirstOrDefault<System_ConfigurationInfo>(o => o.Category == "SystemDataProperty");
                if (dataPropertyConfig != null)
                {
                    UnitWork.Update<System_ConfigurationInfo>(u => u.Id == dataPropertyConfig.Id, u => new System_ConfigurationInfo
                    {
                        Text = obj.Code
                    });
                }
            }
            UpdateTreeObj(obj);
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="req"></param>
        public void Delete(ModuleDeleteReq req)
        {
            var loginContext = _auth.GetCurrentUser();
            if (loginContext == null)
            {
                throw new InfoException("登录已过期", Define.INVALID_TOKEN);
            }
            if (string.IsNullOrEmpty(req.Id))
            {
                throw new InfoException("模块Id不能为空");
            }

            UnitWork.ExecuteWithTransaction(() =>
            {
                var moduleElements = UnitWork.Find<Auth_ModuleElementInfo>(o => req.Id == o.ModuleId);
                var moduleElementIds = moduleElements.Select(o => o.Id);
                //删除菜单
                UnitWork.Delete<Auth_ModuleElementInfo>(o => req.Id == o.ModuleId);
                //删除菜单角色关联
                UnitWork.Delete<Auth_RelevanceInfo>(o => moduleElementIds.Contains(o.SecondId) && o.Key == Define.ROLEELEMENT);
                //删除模块角色关联
                UnitWork.Delete<Auth_RelevanceInfo>(o => req.Id == o.SecondId && o.Key == Define.ROLEMODULE);
                //删除模块
                UnitWork.Delete<Auth_ModuleInfo>(o => o.Id == req.Id);
                UnitWork.Save();
            });
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
        /// <param name="req"></param>
        /// <returns></returns>
        public IList<LoadPropertiesForRoleView> LoadPropertiesForRole(LoadPropertiesForRoleReq req)
        {
            if (string.IsNullOrEmpty(req.RoleId))
            {
                throw new InfoException("角色Id不能为空");
            }
            if (req.ModuleIds == null || req.ModuleIds.Count() == 0)
            {
                return new List<LoadPropertiesForRoleView>();
            }
            return _app.GetRoleProp(Define.ROLEDATAPROPERTY, req.RoleId, req.ModuleIds);
        }

        /// <summary>
        /// 根据某角色ID获取可访问某模块的菜单项
        /// </summary>
        public IEnumerable<string> LoadMenusForRole(LoadMenusForRoleReq req)
        {
            var elementIds = _app.Get(Define.ROLEELEMENT, req.RoleId, req.ModuleIds);
            return elementIds;
        }

        #endregion 用户/角色分配模块

        #region 菜单操作

        /// <summary>
        /// 删除指定的菜单
        /// </summary>
        /// <param name="req"></param>
        public void DelMenu(DelMenuReq req)
        {
            var loginContext = _auth.GetCurrentUser();
            if (loginContext == null)
            {
                throw new InfoException("登录已过期", Define.INVALID_TOKEN);
            }
            if (string.IsNullOrEmpty(req.Id))
            {
                throw new InfoException("菜单Id不能为空");
            }

            UnitWork.ExecuteWithTransaction(() =>
            {
                var moduleElement = UnitWork.Find<Auth_ModuleElementInfo>(o => req.Id == o.Id);
                if (moduleElement == null)
                {
                    throw new InfoException($"没有找到菜单Id为：{req.Id}的数据");
                }
                //删除菜单
                UnitWork.Delete<Auth_ModuleElementInfo>(o => o.Id == req.Id);
                //删除菜单角色关联
                UnitWork.Delete<Auth_RelevanceInfo>(o => req.Id == o.ThirdId && o.Key == Define.ROLEELEMENT);
                UnitWork.Save();
            });
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
                    _app.Assign(Define.ROLEELEMENT, u.Id, model.ModuleId, new string[] { model.Id });
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
