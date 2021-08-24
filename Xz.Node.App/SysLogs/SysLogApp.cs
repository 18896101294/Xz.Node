using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xz.Node.App.Base;
using Xz.Node.App.Request;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.SysLogs
{
    /// <summary>
    /// 系统日志管理
    /// </summary>
    public class SysLogApp : BaseStringApp<System_SysLogInfo, XzDbContext>
    {
        public SysLogApp(IUnitWork<XzDbContext> unitWork, IRepository<System_SysLogInfo, XzDbContext> repository) : base(unitWork, repository, null)
        {

        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PageInfo<System_SysLogInfo> GetPageData(BaseDto.PageDataModel dto)
        {
            PageInfo<System_SysLogInfo> page = new PageInfo<System_SysLogInfo>()
            {
                PageIndex = dto.PageIndex ?? 1,
                PageSize = dto.PageSize ?? 20
            };
            Repository.GetPageDatas(dto.Conditions.ToConditions(), dto.Sorts.ToSorts(), page);
            return page;
        }

        /// <summary>
        /// 加载列表，不推荐的写法，效率低下
        /// </summary>
        [Obsolete("替代方法：Repository.GetPageDatas")]
        public PageInfo<System_SysLogInfo> Load(QuerySysLogListReq request)
        {
            var result = new PageInfo<System_SysLogInfo>();

            var objs = UnitWork.Find<System_SysLogInfo>(null);
            if (!string.IsNullOrEmpty(request.key))
            {
                objs = objs.Where(u => u.Content.Contains(request.key) || u.Id.Contains(request.key));
            }
            if (!string.IsNullOrEmpty(request.TypeName))
            {
                objs = objs.Where(u => u.TypeName == request.TypeName);
            }
            if (!string.IsNullOrEmpty(request.CreateName))
            {
                objs = objs.Where(u => u.CreateName == request.CreateName);
            }
            if (!string.IsNullOrEmpty(request.Ip))
            {
                objs = objs.Where(u => u.Ip == request.Ip);
            }
            if (request.BeginCreateTime.HasValue && request.EndCreateTime.HasValue)
            {
                objs = objs.Where(u => u.CreateTime >= request.BeginCreateTime && u.CreateTime <= request.EndCreateTime);
            }
            if (request.Result.HasValue)
            {
                objs = objs.Where(u => u.Result == request.Result);
            }

            result.PageIndex = request.page;
            result.PageSize = request.limit;
            result.Total = objs.Count();
            result.Datas = objs.OrderByDescending(u => u.CreateTime)
                .Skip((request.page - 1) * request.limit)
                .Take(request.limit).ToList();
            return result;
        }

        /// <summary>
        /// 添加系统日志
        /// </summary>
        /// <param name="obj"></param>
        public void Add(System_SysLogInfo obj)
        {
            //程序类型取入口应用的名称，可以根据自己需要调整
            obj.Application = Assembly.GetEntryAssembly().FullName.Split(',')[0];
            Repository.Add(obj);
        }

        /// <summary>
        /// 修改系统日志
        /// </summary>
        /// <param name="obj"></param>
        public void Update(System_SysLogInfo obj)
        {
            UnitWork.Update<System_SysLogInfo>(u => u.Id == obj.Id, u => new System_SysLogInfo
            {
                //todo:要修改的字段赋值
            });
        }

        /// <summary>
        /// 清理10天前的日志数据
        /// </summary>
        public void ClearData()
        {
            UnitWork.ExecuteWithTransaction(() =>
            {
                UnitWork.Delete<System_SysLogInfo>(o => o.CreateTime < DateTime.Now.Date.AddDays(-10));
                UnitWork.Save();
            });
        }
    }
}