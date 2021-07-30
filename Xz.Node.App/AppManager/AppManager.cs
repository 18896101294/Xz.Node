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
    /// 分类管理
    /// </summary>
    public class AppManager : BaseStringApp<System_ApplicationInfo, XzDbContext>
    {
        public AppManager(IUnitWork<XzDbContext> unitWork, IRepository<System_ApplicationInfo, XzDbContext> repository) : base(unitWork, repository, null)
        {

        }

        public void Add(System_ApplicationInfo Application)
        {
            if (string.IsNullOrEmpty(Application.Id))
            {
                Application.Id = Guid.NewGuid().ToString();
            }

            Repository.Add(Application);
        }

        public void Update(System_ApplicationInfo Application)
        {
            Repository.Update(Application);
        }

        public async Task<List<System_ApplicationInfo>> GetList(QueryAppListReq request)
        {
            var applications = UnitWork.Find<System_ApplicationInfo>(null);

            return applications.ToList();
        }

        public System_ApplicationInfo GetByAppKey(string modelAppKey)
        {
            return Repository.FirstOrDefault(u => u.AppSecret == modelAppKey);
        }
    }
}