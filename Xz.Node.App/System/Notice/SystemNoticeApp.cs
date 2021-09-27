using System;
using System.Collections.Generic;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.App.System.Notice.Request;
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
        /// <summary>
        /// 系统配置构造函数
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public SystemNoticeApp(IUnitWork<XzDbContext> unitWork, IRepository<System_NoticeInfo, XzDbContext> repository, IAuth auth) : base(unitWork, repository, auth)
        {
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
            Repository.Update(o => req.Id == o.Id, o => new System_NoticeInfo { IsExec = false, ExecTime = req.ExecTime });
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
            if(oldData == null)
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
    }
}
