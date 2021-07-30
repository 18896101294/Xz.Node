using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xz.Node.Framework.Model;

namespace Xz.Node.Repository.Interface
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IRepository<T,TDbContext> where T : class where TDbContext: DbContext
    {
        /// <summary>
        /// 查询
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 返回一个单独的实体，如果记录多于1个则取第一个
        /// </summary>
        T FirstOrDefault(Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 判断指定条件的记录是否存在
        /// </summary>
        bool Any(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 指定条件查找实体
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        IQueryable<T> Find(Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 分页查看实体
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="orderby"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        IQueryable<T> Find(int pageindex = 1, int pagesize = 10, string orderby = "",
            Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 获取指定条件的数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        IList<T> GetDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts, bool? isDelete = false);

        /// <summary>
        /// 获取指定条数、指定条件的数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        IList<T> GetTopDatas(int top = 10, IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null, bool? isDelete = false);

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        int GetCount(IList<ConditionInfo> conditions = null, bool? isDelete = false);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <param name="isDelete"></param>
        void GetPageDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts, PageInfo<T> page, bool? isDelete = false);

        /// <summary>
        /// 添加一个实体
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities"></param>
        void BatchAdd(T[] entities);

        /// <summary>
        /// 更新一个实体的所有属性
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="entitys"></param>
        void Update(IList<T> entitys);

        /// <summary>
        /// 物理删除实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">更新条件</param>
        /// <param name="entity">更新后的实体</param>
        void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        /// <summary>
        /// 批量物理删除
        /// </summary>
        void Delete(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 执行sql脚本
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSqlRaw(string sql);

        /// <summary>
        /// 使用SQL脚本查询
        /// </summary>
        /// <typeparam name="T"> T为数据库实体</typeparam>
        /// <returns></returns>
        IQueryable<T> FromSql(string sql, params object[] parameters);

        /// <summary>
        /// 使用SQL脚本查询
        /// </summary>
        /// <typeparam name="T"> T为非数据库实体，需要在DbContext中增加对应的DbQuery</typeparam>
        /// <returns></returns>
        IQueryable<T> Query(string sql, params object[] parameters);

        /// <summary>
        /// 提交数据库
        /// </summary>
        void Save();

        #region 异步接口
        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlRawAsync(string sql);

        /// <summary>
        /// 异步添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> AddAsync(T entity);

        /// <summary>
        /// 异步批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> BatchAddAsync(T[] entities);

        /// <summary>
        /// 异步修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// 异步删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity);

        /// <summary>
        /// 异步提交
        /// </summary>
        /// <returns></returns>
        Task<int> SaveAsync();

        /// <summary>
        /// 异步获取条数
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> exp = null);

        /// <summary>
        /// 异步判断是否存在
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 异步获取第一条
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp);
        #endregion
    }
}