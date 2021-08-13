using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.System.Configuration
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfigurationApp : BaseGuidApp<System_ConfigurationInfo, XzDbContext>
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
        public SystemConfigurationApp(IUnitWork<XzDbContext> unitWork, IRepository<System_ConfigurationInfo, XzDbContext> repository, IAuth auth,
            ICacheContext cacheContext) : base(unitWork, repository, auth)
        {
            _cacheContext = cacheContext;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PageInfo<System_ConfigurationInfo> GetPageData(BaseDto.PageDataModel dto)
        {
            var data = base.GetPageDatas(dto.Conditions.ToConditions(), dto.Sorts.ToSorts(), dto.PageIndex ?? 1, dto.PageSize ?? 20);
            return data;
        }

        /// <summary>
        /// 根据参数类型获取参数值
        /// </summary>
        /// <param name="category">参数类型</param>
        /// <returns></returns>
        public IList<System_ConfigurationInfo> GetSysConfigurations(string category)
        {
            var query = GetAllDatas().Where(o => o.Category == category);
            query = query.Where(o => !o.IsHide);
            var datas = query.OrderBy(o => o.DisplayNo).ToList();
            return datas;
        }

        /// <summary>
        /// 获取所有的数据
        /// </summary>
        /// <returns></returns>
        public IList<System_ConfigurationInfo> GetAllDatas()
        {
            var datas = _cacheContext.Get<IList<System_ConfigurationInfo>>(Define.SystemConfigurationCacheKey);
            if (datas == null)
            {
                lock (_lockObj)
                {
                    datas = _cacheContext.Get<IList<System_ConfigurationInfo>>(Define.SystemConfigurationCacheKey);
                    if (datas == null)
                    {
                        datas = base.GetDatas().OrderBy(o => o.DisplayNo).ToList();
                        if (datas != null && datas.Count() > 0)
                        {
                            _cacheContext.Set(Define.SystemConfigurationCacheKey, datas, null);
                        }
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAllCategory()
        {
            var resultData = new List<string>();
            var datas = this.GetAllDatas();
            if (datas != null)
            {
                resultData = datas.Select(o => o.Category).Distinct().ToList();
            }
            return resultData;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public void DeleteData(IList<Guid> ids)
        {
            Repository.Update(o => o.IsDelete == false && ids.Contains(o.Id), o => new System_ConfigurationInfo { IsDelete = true });
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
        protected override void AfterInsert(System_ConfigurationInfo model)
        {
            RemoveCache();
        }

        /// <summary>
        /// 修改后清除缓存
        /// </summary>
        /// <param name="model"></param>
        protected override void AfterUpdate(System_ConfigurationInfo model)
        {
            RemoveCache();
        }

        /// <summary>
        /// 删除缓存的数据
        /// </summary>
        private void RemoveCache()
        {
            _cacheContext.Remove(Define.SystemConfigurationCacheKey);
        }
    }
}
