using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Core;
using Xz.Node.Repository.Interface;
using Xz.Node.Framework.Common;
using Z.EntityFramework.Plus;

namespace Xz.Node.Repository
{
    /// <summary>
    /// 数据层基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class BaseRepository<T, TDbContext> : IRepository<T, TDbContext> where T : BaseEntity where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        private readonly ICacheContext _cacheContext;
        /// <summary>
        /// 数据层基类
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheContext"></param>
        public BaseRepository(TDbContext context, ICacheContext cacheContext)
        {
            _context = context;
            _cacheContext = cacheContext;
        }

        private DbSet<T> Entity => _context.Set<T>();

        /// <summary>
        /// 获取数据对象
        /// </summary>
        public IQueryable<T> Table => _context.Set<T>().AsNoTracking();

        /// <summary>
        /// 根据过滤条件，获取记录
        /// </summary>
        /// <param name="exp">The exp.</param>
        public IQueryable<T> Find(Expression<Func<T, bool>> exp = null)
        {
            return Filter(exp);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().Any(exp);
        }

        /// <summary>
        /// 查找单个，且不被上下文所跟踪
        /// </summary>
        public T FirstOrDefault(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().AsNoTracking().FirstOrDefault(exp);
        }

        /// <summary>
        /// 得到分页记录
        /// </summary>
        /// <param name="pageindex">The pageindex.</param>
        /// <param name="pagesize">The pagesize.</param>
        /// <param name="orderby">排序，格式如："Id"/"Id descending"</param>
        /// <param name="exp">linq表达式</param>
        public IQueryable<T> Find(int pageindex, int pagesize, string orderby = "", Expression<Func<T, bool>> exp = null)
        {
            if (pageindex < 1)
                pageindex = 1;
            if (string.IsNullOrEmpty(orderby))
                orderby = "Id descending";
            return Filter(exp).OrderBy(orderby).Skip(pagesize * (pageindex - 1)).Take(pagesize);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public IList<T> GetDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts, bool? isDelete = false)
        {
            conditions = FiltrationCondition(conditions);
            conditions = DelCondition(conditions, isDelete);

            sorts = FiltrationSort(sorts);
            var expression = EFSQLHelpr.QueryConditionCreate<T>(conditions);
            var query = Entity.AsNoTracking().Where(expression);
            if (sorts != null && sorts.Count() > 0)
            {
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            else
            {
                sorts = new List<SortInfo> {
                    new SortInfo
                    {
                       ColumnName="CreateTime",
                       Direction=ConditionDirectionEnum.DESC
                    }
                };
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            var datas = query.ToList();
            return datas;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <param name="isDelete">是否删除, 默认false</param>
        public virtual void GetPageDatas(IList<ConditionInfo> conditions, IList<SortInfo> sorts, PageInfo<T> page, bool? isDelete = false)
        {
            page.PageIndex = page.PageIndex <= 0 ? 1 : page.PageIndex;
            page.PageSize = page.PageSize <= 0 ? 20 : page.PageSize;

            conditions = FiltrationCondition(conditions);
            conditions = DelCondition(conditions, isDelete);

            sorts = FiltrationSort(sorts);

            var expression = EFSQLHelpr.QueryConditionCreate<T>(conditions);
            var query = Entity.AsNoTracking().Where(expression);
            if (sorts != null || sorts.Count > 0)
            {
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            else
            {
                sorts = new List<SortInfo>
                {
                    new SortInfo
                    {
                       ColumnName="CreateTime",
                       Direction=ConditionDirectionEnum.DESC
                    }
                };
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            page.Total = query.Count();
            page.Datas = query.Skip((page.PageIndex - 1) * page.PageSize).Take(page.PageSize).ToList();
        }

        /// <summary>
        /// 获取top数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public virtual IList<T> GetTopDatas(int top = 10, IList<ConditionInfo> conditions = null, IList<SortInfo> sorts = null, bool? isDelete = false)
        {
            conditions = FiltrationCondition(conditions);
            conditions = DelCondition(conditions, isDelete);

            sorts = FiltrationSort(sorts);

            var expression = EFSQLHelpr.QueryConditionCreate<T>(conditions);
            var query = Entity.AsNoTracking().Where(expression);
            if (sorts != null || sorts.Count > 0)
            {
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            else
            {
                sorts = new List<SortInfo>
                {
                    new SortInfo
                    {
                       ColumnName="CreateTime",
                       Direction=ConditionDirectionEnum.DESC
                    }
                };
                query = EFSQLHelpr.SortConditionCreate<T>(query, sorts);
            }
            var datas = query.Take(top).ToList();
            return datas;
        }

        /// <summary>
        /// 获取条数
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        public virtual int GetCount(IList<ConditionInfo> conditions = null, bool? isDelete = false)
        {
            conditions = FiltrationCondition(conditions);
            conditions = DelCondition(conditions, isDelete);

            var expression = EFSQLHelpr.QueryConditionCreate<T>(conditions);
            int count = Entity.AsNoTracking().Where(expression).Count();
            return count;
        }

        /// <summary>
        /// 过滤条件
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        protected IList<ConditionInfo> FiltrationCondition(IList<ConditionInfo> conditions)
        {
            //conditions = conditions.Where(s => !string.IsNullOrEmpty(s.Value)).ToList();
            if (conditions != null && conditions.Count > 0)
            {
                foreach (var item in conditions)
                {
                    item.ColumnName = ConvertPropertyName(item.ColumnName);
                }
            }
            return conditions;
        }

        /// <summary>
        /// 过滤条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IList<ConditionInfo> FiltrationCondition<T>(IList<ConditionInfo> conditions)
        {
            conditions = conditions.Where(s => !string.IsNullOrEmpty(s.Value)).ToList();
            if (conditions != null && conditions.Count > 0)
            {
                foreach (var item in conditions)
                {
                    item.ColumnName = ConvertPropertyName<T>(item.ColumnName);
                }
            }
            return conditions;
        }

        /// <summary>
        /// 过滤删除
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        protected virtual IList<ConditionInfo> DelCondition(IList<ConditionInfo> conditions, bool? isDelete)
        {
            if (isDelete.HasValue)
            {
                conditions = conditions ?? new List<ConditionInfo>();
                var deleteCondition = conditions.Where(o => o.ColumnName.ToUpper() == "IsDelete").FirstOrDefault();
                if (deleteCondition == null)
                {
                    conditions.Add(new ConditionInfo()
                    {
                        ColumnName = "IsDelete",
                        Value = isDelete.Value.ToString()
                    });
                }
                else
                {
                    deleteCondition.Value = isDelete.Value.ToString();
                }
            }
            return conditions;
        }

        /// <summary>
        /// 过滤排序
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns></returns>
        protected IList<SortInfo> FiltrationSort(IList<SortInfo> sorts)
        {
            if (sorts != null && sorts.Count > 0)
            {
                foreach (var item in sorts)
                {
                    item.ColumnName = ConvertPropertyName(item.ColumnName);
                }
            }
            return sorts;
        }

        /// <summary>
        /// 过滤排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public IList<SortInfo> FiltrationSort<T>(IList<SortInfo> sorts)
        {
            if (sorts != null && sorts.Count > 0)
            {
                foreach (var item in sorts)
                {
                    item.ColumnName = ConvertPropertyName<T>(item.ColumnName);
                }
            }
            return sorts;
        }

        /// <summary>
        /// 根据过滤条件获取记录数
        /// </summary>
        public int Count(Expression<Func<T, bool>> exp = null)
        {
            return Filter(exp).Count();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            if (entity.KeyIsNull())
            {
                entity.GenerateDefaultKeyVal();
            }
            var entry = _context.Set<T>().Add(entity);

            if (entry.Properties.Where(s => s.Metadata.Name.ToLower() == "CreateTime".ToLower()).FirstOrDefault() != null)
                entry.Property("CreateTime").IsModified = false;
            if (entry.Properties.Where(s => s.Metadata.Name.ToLower() == "CreateUserId".ToLower()).FirstOrDefault() != null)
                entry.Property("CreateUserId").IsModified = false;
            if (entry.Properties.Where(s => s.Metadata.Name.ToLower() == "Creater".ToLower()).FirstOrDefault() != null)
                entry.Property("Creater").IsModified = false;
            Save();
            _context.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void BatchAdd(T[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity.KeyIsNull())
                {
                    entity.GenerateDefaultKeyVal();
                }
            }
            _context.Set<T>().AddRange(entities);
            Save();
        }

        /// <summary>
        /// 修改（修改所有属性）
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            var entry = this._context.Entry(entity);
            entry.State = EntityState.Modified;
            //如果数据没有发生变化
            if (!this._context.ChangeTracker.HasChanges())
            {
                return;
            }
            Save();
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="entitys"></param>
        public void Update(IList<T> entitys)
        {
            foreach (var entity in entitys)
            {
                var entry = this._context.Entry(entity);
                entry.State = EntityState.Modified;
            }
            Save();
        }

        /// <summary>
        /// 实现按需要只部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        public void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            _context.Set<T>().Where(where).Update(entity);
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            Save();
        }

        /// <summary>
        /// 条件物理删除
        /// </summary>
        /// <param name="exp"></param>
        public virtual void Delete(Expression<Func<T, bool>> exp)
        {
            _context.Set<T>().Where(exp).Delete();
        }

        /// <summary>
        /// 提交数据库
        /// </summary>
        public void Save()
        {
            try
            {
                var entities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity);

                foreach (var entity in entities)
                {
                    var validationContext = new ValidationContext(entity);
                    Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
                }
                _context.SaveChanges();
            }
            catch (ValidationException exc)
            {
                Console.WriteLine($"{nameof(Save)} validation exception: {exc?.Message}");
                throw (exc.InnerException as Exception ?? exc);
            }
            catch (Exception ex) //DbUpdateException 
            {
                throw (ex.InnerException as Exception ?? ex);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSqlRaw(string sql)
        {
            return _context.Database.ExecuteSqlRaw(sql);
        }

        /// <summary>
        /// 使用SQL脚本查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IQueryable<T> FromSql(string sql, params object[] parameters)
        {
            return _context.Set<T>().FromSqlRaw(sql, parameters);
        }

        /// <summary>
        /// 使用SQL脚本查询
        ///  T为非数据库实体，需要在DbContext中增加对应的DbQuery
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete]
        public IQueryable<T> Query(string sql, params object[] parameters)
        {
            return _context.Query<T>().FromSqlRaw(sql, parameters);
        }

        #region 异步实现

        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlRawAsync(string sql)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql);
        }

        /// <summary>
        /// 异步添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(T entity)
        {
            if (entity.KeyIsNull())
            {
                entity.GenerateDefaultKeyVal();
            }

            _context.Set<T>().Add(entity);
            return await SaveAsync();
            //_context.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// 异步批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<int> BatchAddAsync(T[] entities)
        {
            foreach (var entity in entities)
            {
                if (entity.KeyIsNull())
                {
                    entity.GenerateDefaultKeyVal();
                }
            }
            await _context.Set<T>().AddRangeAsync(entities);
            return await SaveAsync();
        }

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(T entity)
        {
            var entry = this._context.Entry(entity);
            entry.State = EntityState.Modified;

            //如果数据没有发生变化
            if (!this._context.ChangeTracker.HasChanges())
            {
                return 0;
            }
            return await SaveAsync();
        }

        /// <summary>
        /// 异步删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await SaveAsync();
        }

        /// <summary>
        /// 异步保存
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> SaveAsync()
        {
            try
            {
                var entities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => e.Entity);

                foreach (var entity in entities)
                {
                    var validationContext = new ValidationContext(entity);
                    Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
                }
                return await _context.SaveChangesAsync();
            }
            catch (ValidationException exc)
            {
                Console.WriteLine($"{nameof(Save)} validation exception: {exc?.Message}");
                throw (exc.InnerException as Exception ?? exc);
            }
            catch (Exception ex) //DbUpdateException 
            {
                throw (ex.InnerException as Exception ?? ex);
            }
        }

        /// <summary>
        /// 根据过滤条件获取记录数
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>> exp = null)
        {
            return await Filter(exp).CountAsync();
        }

        /// <summary>
        /// 异步判断是否存在
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> exp)
        {
            return await _context.Set<T>().AnyAsync(exp);
        }

        /// <summary>
        /// 查找单个，且不被上下文所跟踪
        /// </summary>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(exp);
        }
        #endregion

        #region private
        /// <summary>
        /// 转换属性名称，需要兼容（1.将字段名转成属性名称；2.将大小写不规范的属性名称，转成大小写规范的属性名称）
        /// </summary>
        private string ConvertPropertyName(string propertyName)
        {
            return ConvertPropertyName<T>(propertyName);
        }

        private string ConvertPropertyName<T>(string propertyName)
        {
            var currPropertyName = propertyName;
            propertyName = propertyName.ToLower();
            var tModelType = typeof(T);
            Dictionary<string, string> keyValues = _cacheContext.Get<Dictionary<string, string>>("Dal_Model_" + tModelType.FullName);
            if (keyValues == null)
            {
                keyValues = new Dictionary<string, string>();
                foreach (PropertyInfo p in tModelType.GetProperties())
                {
                    var attributes = p.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (attributes.Length > 0)
                    {
                        ColumnAttribute attribute = (ColumnAttribute)attributes[0];
                        keyValues.Add(attribute.Name.ToLower(), p.Name);
                    }
                }
                _cacheContext.Set("Dal_Model_" + tModelType.FullName, keyValues, DateTime.Now.AddDays(1));
            }

            if (keyValues.ContainsKey(propertyName))
            {
                currPropertyName = keyValues[propertyName];
            }
            else if (keyValues.Where(o => o.Value.ToLower() == propertyName).Any()) //兼容大小写问题
            {
                currPropertyName = keyValues.Where(o => o.Value.ToLower() == propertyName).Select(o => o.Value).FirstOrDefault();
            }
            return currPropertyName;
        }

        private IQueryable<T> Filter(Expression<Func<T, bool>> exp)
        {
            var dbSet = _context.Set<T>().AsNoTracking().AsQueryable();
            if (exp != null)
                dbSet = dbSet.Where(exp);
            return dbSet;
        }
        #endregion
    }
}