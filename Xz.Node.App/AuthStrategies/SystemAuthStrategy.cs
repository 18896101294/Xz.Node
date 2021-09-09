using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
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
        private DbExtension _dbExtension;
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
        /// <param name="className"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<KeyDescription> GetClassProperties(string className, string moduleId)
        {
            var asm = Assembly.GetExecutingAssembly();
            Type type = null;
            foreach (var typeItem in asm.GetTypes())
            {
                if (typeItem.Name.ToLower().Equals(className.ToLower()))
                {
                    type = typeItem;
                }
            }
            if (type == null)
            {
                throw new InfoException("获取数据字典失败");
            }
            var properties = type.GetProperties().ToList();

            var result = new List<KeyDescription>();

            foreach (var property in properties)
            {
                object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                object[] browsableObjs = property.GetCustomAttributes(typeof(BrowsableAttribute), true);
                var description = objs.Length > 0 ? ((DescriptionAttribute)objs[0]).Description : property.Name;
                if (string.IsNullOrEmpty(description)) description = property.Name;
                //如果没有BrowsableAttribute或 [Browsable(true)]表示可见，其他均为不可见，需要前端配合显示
                bool browsable = browsableObjs == null || browsableObjs.Length == 0 ||
                                 ((BrowsableAttribute)browsableObjs[0]).Browsable;
                var typeName = property.PropertyType.Name;
                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                {
                    typeName = Nullable.GetUnderlyingType(property.PropertyType).Name;
                }
                result.Add(new KeyDescription
                {
                    Key = property.Name,
                    Description = description,
                    Browsable = browsable,
                    Type = typeName
                });
            }
            return result;
        }
    }
}