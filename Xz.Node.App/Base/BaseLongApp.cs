using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xz.Node.App.Interface;
using Xz.Node.Repository.Core;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// 数据库Id为numberic类型的数据表相关业务使用该基类
    /// 业务层基类，UnitWork用于事务操作，Repository用于普通的数据库操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class BaseLongApp<T, TDbContext> : BaseApp<T, TDbContext> where T : LongEntity where TDbContext : DbContext
    {
        /// <summary>
        /// 数据库Id为numberic类型的数据表相关业务使用该基类构造
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public BaseLongApp(IUnitWork<TDbContext> unitWork, IRepository<T, TDbContext> repository, IAuth auth) : base(unitWork, repository, auth)
        {
        }

        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void Delete(long[] ids)
        {
            Repository.Delete(u => ids.Contains(u.Id));
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(long id)
        {
            return Repository.FirstOrDefault(u => u.Id == id);
        }
    }
}