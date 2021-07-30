using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xz.Node.Framework.Model;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// Guid业务层基类接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseGuidApp<T>
    {
        #region 查询
        /// <summary>
        /// 获取实例化对象
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        T Get(Guid id, bool? isDelete = false);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        IList<T> GetDatas(IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null,
          bool? isDelete = false);

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        PageInfo<T> GetPageDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts,
         int pageIndex, int pageSize, bool? isDelete = false);

        /// <summary>
        /// 获取top数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        IList<T> GetTopDatas(int top = 10, IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null, bool? isDelete = false);
        #endregion

        #region 添加
        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="model"></param>
        void Insert(T model);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="models"></param>
        void Insert(IList<T> models);
        #endregion

        #region 修改
        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="model"></param>
        void Update(T model);
        #endregion

        #region 删除
        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        void Delete(IList<Guid> ids);
        #endregion

        #region 导入
        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <returns></returns>
        byte[] GetImportTemplate();

        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="file"></param>
        void Import(Stream file);
        #endregion

        #region 导出
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="conditionInfos"></param>
        /// <param name="sortInfos"></param>
        /// <returns></returns>
        byte[] Export(IList<ConditionInfo> conditionInfos, IList<SortInfo> sortInfos = null);

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <param name="conditionInfos"></param>
        /// <param name="sortInfos"></param>
        /// <returns></returns>
        dynamic GetExportDatas(IList<ConditionInfo> conditionInfos, IList<SortInfo> sortInfos);
        #endregion

        #region Common
        /// <summary>
        /// 提交
        /// </summary>
        void SaveChange();
        #endregion
    }
}
