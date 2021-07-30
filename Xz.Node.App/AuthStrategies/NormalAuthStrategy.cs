using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.App.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.AuthStrategies
{
    /// <summary>
    /// 普通用户授权策略
    /// </summary>
    public class NormalAuthStrategy : BaseStringApp<Auth_UserInfo, XzDbContext>, IAuthStrategy
    {

        protected Auth_UserInfo _user;
        private List<string> _userRoleIds;    //用户角色GUID
        private DbExtension _dbExtension;
        public NormalAuthStrategy(IUnitWork<XzDbContext> unitWork, IRepository<Auth_UserInfo, XzDbContext> repository, DbExtension dbExtension) : base(unitWork, repository, null)
        {
            _dbExtension = dbExtension;
        }

        public List<ModuleView> Modules
        {
            get
            {
                var moduleIds = UnitWork.Find<Auth_RelevanceInfo>(
                    u =>
                        (u.Key == Define.ROLEMODULE && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);

                var modules = (from module in UnitWork.Find<Auth_ModuleInfo>(u => moduleIds.Contains(u.Id))
                               select new ModuleView
                               {
                                   SortNo = module.SortNo,
                                   Name = module.Name,
                                   Code = module.Code,
                                   CascadeId = module.CascadeId,
                                   Id = module.Id,
                                   IconName = module.IconName,
                                   Url = module.Url,
                                   ParentId = module.ParentId,
                                   ParentName = module.ParentName,
                                   IsSys = module.IsSys,
                                   Status = module.Status
                               }).ToList();
                var usermoduleelements = ModuleElements;
                foreach (var module in modules)
                {
                    module.Elements = usermoduleelements.Where(u => u.ModuleId == module.Id).ToList();
                }
                return modules;
            }
        }

        public List<Auth_ModuleElementInfo> ModuleElements
        {
            get
            {
                var elementIds = UnitWork.Find<Auth_RelevanceInfo>(u => (u.Key == Define.ROLEELEMENT && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId).ToList();
                var usermoduleelements = UnitWork.Find<Auth_ModuleElementInfo>(u => elementIds.Contains(u.Id));
                return usermoduleelements.ToList();
            }
        }

        public List<Auth_RoleInfo> Roles
        {
            get { return UnitWork.Find<Auth_RoleInfo>(u => _userRoleIds.Contains(u.Id)).ToList(); }
        }

        public List<Auth_ResourceInfo> Resources
        {
            get
            {
                var resourceIds = UnitWork.Find<Auth_RelevanceInfo>(
                    u =>
                        (u.Key == Define.ROLERESOURCE && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
                return UnitWork.Find<Auth_ResourceInfo>(u => resourceIds.Contains(u.Id)).ToList();
            }
        }

        public List<Auth_OrgInfo> Orgs
        {
            get
            {
                var orgids = UnitWork.Find<Auth_RelevanceInfo>(
                    u => u.FirstId == _user.Id && u.Key == Define.USERORG).Select(u => u.SecondId);
                return UnitWork.Find<Auth_OrgInfo>(u => orgids.Contains(u.Id)).ToList();
            }
        }

        public Auth_UserInfo User
        {
            get { return _user; }
            set
            {
                _user = value;
                _userRoleIds = UnitWork.Find<Auth_RelevanceInfo>(u => u.FirstId == _user.Id && u.Key == Define.USERROLE).Select(u => u.SecondId).ToList();
            }
        }

        /// <summary>
        /// 获取用户可访问的字段列表
        /// </summary>
        /// <param name="moduleCode">模块的code</param>
        /// <returns></returns>
        public List<KeyDescription> GetProperties(string moduleCode)
        {
            var allprops = _dbExtension.GetProperties(moduleCode);
            //如果是系统模块，直接返回所有字段。防止开发者把模块配置成系统模块，还在外层调用loginContext.GetProperties("xxxx");
            bool? isSysModule = UnitWork.FirstOrDefault<Auth_ModuleInfo>(u => u.Code == moduleCode)?.IsSys;
            if (isSysModule != null && isSysModule.Value)
            {
                return allprops;
            }
            var props = UnitWork.Find<Auth_RelevanceInfo>(u =>
                     u.Key == Define.ROLEDATAPROPERTY && _userRoleIds.Contains(u.FirstId) && u.SecondId == moduleCode)
                .Select(u => u.ThirdId);
            return allprops.Where(u => props.Contains(u.Key)).ToList();
        }
    }
}