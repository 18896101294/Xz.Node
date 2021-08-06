using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.AppManagers.Request;
using Xz.Node.App.Base;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.AppManagers
{
    /// <summary>
    /// 应用管理
    /// </summary>
    public class AppManager : BaseStringApp<System_ApplicationInfo, XzDbContext>
    {
        /// <summary>
        /// 应用管理
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        public AppManager(IUnitWork<XzDbContext> unitWork, IRepository<System_ApplicationInfo, XzDbContext> repository) : base(unitWork, repository, null)
        {

        }

        /// <summary>
        /// 添加应用
        /// </summary>
        /// <param name="Application"></param>
        public void Add(System_ApplicationInfo Application)
        {
            if (string.IsNullOrEmpty(Application.Id))
            {
                Application.Id = Guid.NewGuid().ToString();
            }

            Repository.Add(Application);
        }

        /// <summary>
        /// 修改应用
        /// </summary>
        /// <param name="Application"></param>
        public void Update(System_ApplicationInfo Application)
        {
            Repository.Update(Application);
        }

        /// <summary>
        /// 获取应用
        /// </summary>
        /// <returns></returns>
        public List<System_ApplicationInfo> GetList()
        {
            var applications = UnitWork.Find<System_ApplicationInfo>(null);
            return applications.ToList();
        }

        /// <summary>
        /// 根据appKey获取应用
        /// </summary>
        /// <param name="modelAppKey"></param>
        /// <returns></returns>
        public System_ApplicationInfo GetByAppKey(string modelAppKey)
        {
            return Repository.FirstOrDefault(u => u.AppSecret == modelAppKey);
        }
    }
}