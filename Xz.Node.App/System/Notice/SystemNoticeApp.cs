using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.App.System.Notice.Request;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.System.Notice
{
    /// <summary>
    /// 系统通知
    /// </summary>
    public class SystemNoticeApp : BaseGuidApp<System_NoticeInfo, XzDbContext>
    {
        private readonly static object _lockObj = new object();
        private readonly ICacheContext _cacheContext;
        /// <summary>
        /// 系统配置构造函数
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        /// <param name="cacheContext"></param>
        public SystemNoticeApp(IUnitWork<XzDbContext> unitWork, IRepository<System_NoticeInfo, XzDbContext> repository, IAuth auth,
            ICacheContext cacheContext) : base(unitWork, repository, auth)
        {
            _cacheContext = cacheContext;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PageInfo<System_NoticeInfo> GetPageData(BaseDto.PageDataModel dto)
        {
            var data = base.GetPageDatas(dto.Conditions.ToConditions(), dto.Sorts.ToSorts(), dto.PageIndex ?? 1, dto.PageSize ?? 20);
            return data;
        }

        /// <summary>
        /// 获取所有的数据
        /// </summary>
        /// <returns></returns>
        public IList<System_NoticeInfo> GetAllDatas()
        {
            var datas = _cacheContext.Get<IList<System_NoticeInfo>>(Define.SystemNoticeCacheKey);
            if (datas == null)
            {
                lock (_lockObj)
                {
                    datas = base.GetDatas().OrderBy(o => o.CreateTime).ToList();
                    if (datas != null && datas.Count() > 0)
                    {
                        _cacheContext.Set(Define.SystemNoticeCacheKey, datas, null);
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取需要执行的通知
        /// </summary>
        /// <returns></returns>
        public IList<System_NoticeInfo> GetExecDats()
        {
            var resultData = new List<System_NoticeInfo>();
            //获取需要执行的系统通知配置
            var noticeDatas = this.GetAllDatas();
            var execDatas = noticeDatas.Where(o => o.IsExec == false && o.Status == 0 && o.IsDelete == false);
            foreach (var data in execDatas)
            {
                if (data.ExecType == 1)
                {
                    resultData.Add(data);
                    continue;
                }
                if (data.ExecType == 2)
                {
                    if (data.ExecTime <= DateTime.Now)
                    {
                        resultData.Add(data);
                    }
                }
            }
            return resultData;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public void DeleteData(BaseIdsReq req)
        {
            Repository.Update(o => o.IsDelete == false && req.Ids.Contains(o.Id.ToString()), o => new System_NoticeInfo { IsDelete = true });
        }

        /// <summary>
        /// 重新执行
        /// </summary>
        /// <param name="req"></param>
        public void ReExecute(ReExecuteReq req)
        {
            if (req.Ids != null && req.Ids.Count() > 0)
            {
                Repository.Update(o => req.Ids.Contains(o.Id), o => new System_NoticeInfo { IsExec = false, ExecTime = req.ExecTime });
                RemoveCache();
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        public override void Insert(System_NoticeInfo obj)
        {
            base.Insert(obj);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="req"></param>
        public void Update(SaveNoticeReq req)
        {
            var oldData = Repository.FirstOrDefault(o => o.Id == req.Id);
            if (oldData == null)
            {
                throw new InfoException("需要修改的数据不存在");
            }
            oldData.Titile = req.Titile;
            oldData.Content = req.Content;
            oldData.Type = req.Type;
            oldData.ExecType = req.ExecType;
            oldData.ExecTime = req.ExecTime;
            oldData.RangeType = req.RangeType;
            oldData.RangeIds = string.Join(',', req.RangeIds);
            oldData.IsHtml = req.IsHtml;
            oldData.Status = req.Status;
            oldData.TenantId = req.TenantId;
            base.Update(oldData);
        }

        /// <summary>
        /// 删除后清除缓存
        /// </summary>
        /// <param name="ids"></param>
        protected override void AfterDelete(IList<Guid> ids)
        {
            RemoveCache();
        }

        /// <summary>
        /// 添加后清除缓存
        /// </summary>
        /// <param name="model"></param>
        protected override void AfterInsert(System_NoticeInfo model)
        {
            RemoveCache();
        }

        /// <summary>
        /// 修改后清除缓存
        /// </summary>
        /// <param name="model"></param>
        protected override void AfterUpdate(System_NoticeInfo model)
        {
            RemoveCache();
        }

        /// <summary>
        /// 删除缓存的数据
        /// </summary>
        private void RemoveCache()
        {
            _cacheContext.Remove(Define.SystemNoticeCacheKey);
        }
    }
}
