using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.AuthStrategies
{
    /// <summary>
    /// 领域服务
    /// <para>超级管理员权限</para>
    /// <para>超级管理员使用guid.empty为ID，可以根据需要修改</para>
    /// </summary>
    public class SystemAuthStrategy : BaseStringApp<Auth_UserInfo, XzDbContext>, IAuthStrategy
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        protected Auth_UserInfo _user;
        private readonly DbExtension _dbExtension;

        /// <summary>
        /// 超级管理员权限
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="dbExtension"></param>
        public SystemAuthStrategy(IUnitWork<XzDbContext> unitWork, IRepository<Auth_UserInfo, XzDbContext> repository, DbExtension dbExtension) : base(unitWork, repository, null)
        {
            _dbExtension = dbExtension;
            _user = new Auth_UserInfo
            {
                Account = Define.SYSTEM_USERNAME,
                Name = "超级管理员",
                Id = Guid.Empty.ToString()
            };
        }

        /// <summary>
        /// 模块数据
        /// </summary>
        public List<ModuleView> Modules
        {
            get
            {
                var modules = (from module in UnitWork.Find<Auth_ModuleInfo>(null)
                               select new ModuleView
                               {
                                   SortNo = module.SortNo,
                                   Name = module.Name,
                                   Id = module.Id,
                                   CascadeId = module.CascadeId,
                                   Code = module.Code,
                                   IconName = module.IconName,
                                   Url = module.Url,
                                   ParentId = module.ParentId,
                                   ParentName = module.ParentName,
                                   IsSys = module.IsSys,
                                   Status = module.Status
                               }).OrderBy(o => o.SortNo).ToList();

                var roleCodes = this.Roles.Select(o => o.Code).ToList();
                foreach (var module in modules)
                {
                    module.Roles = roleCodes;
                    module.Elements = UnitWork.Find<Auth_ModuleElementInfo>(u => u.ModuleId == module.Id).OrderBy(o => o.Sort).ToList();
                }

                return modules;
            }
        }

        /// <summary>
        /// 角色数据
        /// </summary>
        public List<Auth_RoleInfo> Roles
        {
            get { return UnitWork.Find<Auth_RoleInfo>(null).OrderByDescending(o => o.CreateTime).ToList(); }
        }

        /// <summary>
        /// 模块元素数据
        /// </summary>
        public List<Auth_ModuleElementInfo> ModuleElements
        {
            get { return UnitWork.Find<Auth_ModuleElementInfo>(null).ToList(); }
        }

        /// <summary>
        /// 资源数据
        /// </summary>
        public List<Auth_ResourceInfo> Resources
        {
            get { return UnitWork.Find<Auth_ResourceInfo>(null).ToList(); }
        }

        /// <summary>
        /// 组织数据
        /// </summary>
        public List<Auth_OrgInfo> Orgs
        {
            get { return UnitWork.Find<Auth_OrgInfo>(null).ToList(); }
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        public Auth_UserInfo User
        {
            get { return _user; }
            set   //禁止外部设置
            {
                throw new Exception("超级管理员，禁止设置用户");
            }
        }

        /// <summary>
        /// 获取用户可访问的字段列表
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public List<KeyDescription> GetProperties(string moduleCode)
        {
            return _dbExtension.GetProperties(moduleCode);
        }

        /// <summary>
        /// 可访问模块字段集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetClassProperties()
        {
            var resultData = new List<string>();
            var modules = UnitWork.Find<Auth_ModuleInfo>(null).ToList();
            var dataPropertyConfigs = UnitWork.Find<System_ConfigurationInfo>(o => o.Category == "SystemDataProperty").ToList();
            if (modules != null)
            {
                foreach (var module in modules)
                {
                    var dataPropertyConfig = dataPropertyConfigs.FirstOrDefault(o => o.Text == module.Code);
                    if (dataPropertyConfig != null)
                    {
                        var result = _dbExtension.GetKeyDescription(dataPropertyConfig.Value, module.Id);
                        if (result != null && result.Count() > 0)
                        {
                            resultData = result.Select(o => o.Key).ToList();
                        }
                    }
                }
            }
            return resultData;
        }
    }
}