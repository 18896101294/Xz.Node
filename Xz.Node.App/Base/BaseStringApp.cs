using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xz.Node.App.Interface;
using Xz.Node.Repository.Core;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// 业务层基类，UnitWork用于事务操作，Repository用于普通的数据库操作
    /// 如用户管理：Class UserManagerApp:BaseApp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class BaseStringApp<T, TDbContext> :BaseApp<T,TDbContext> where T : StringEntity where TDbContext: DbContext
    {
        /// <summary>
        /// 业务层基类构造
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public BaseStringApp(IUnitWork<TDbContext> unitWork, 
            IRepository<T,TDbContext> repository, 
            IAuth auth) : base(unitWork, repository, auth)
        {
        }

        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        public virtual void Delete(string[] ids)
        {
            Repository.Delete(u => ids.Contains(u.Id));
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(string id)
        {
            return Repository.FirstOrDefault(u => u.Id == id);
        }

    }
}