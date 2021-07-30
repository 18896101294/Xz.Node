using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xz.Node.App.Base;
using Xz.Node.App.Request;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.SysLogs
{
    public class SysLogApp : BaseStringApp<System_SysLogInfo, XzDbContext>
    {
        public SysLogApp(IUnitWork<XzDbContext> unitWork, IRepository<System_SysLogInfo, XzDbContext> repository) : base(unitWork, repository, null)
        {

        }
        /// <summary>
        /// 加载列表
        /// </summary>
        public async Task<TableData> Load(QuerySysLogListReq request)
        {
            var result = new TableData();
            var objs = UnitWork.Find<System_SysLogInfo>(null);
            if (!string.IsNullOrEmpty(request.key))
            {
                objs = objs.Where(u => u.Content.Contains(request.key) || u.Id.Contains(request.key));
            }

            result.data = objs.OrderByDescending(u => u.CreateTime)
                .Skip((request.page - 1) * request.limit)
                .Take(request.limit);
            result.count = objs.Count();
            return result;
        }

        public void Add(System_SysLogInfo obj)
        {
            //程序类型取入口应用的名称，可以根据自己需要调整
            obj.Application = Assembly.GetEntryAssembly().FullName.Split(',')[0];
            Repository.Add(obj);
        }

        public void Update(System_SysLogInfo obj)
        {
            UnitWork.Update<System_SysLogInfo>(u => u.Id == obj.Id, u => new System_SysLogInfo
            {
                //todo:要修改的字段赋值
            });
        }
    }
}