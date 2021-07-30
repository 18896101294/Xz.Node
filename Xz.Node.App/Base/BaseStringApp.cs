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
    /// <para>如用户管理：Class UserManagerApp:BaseApp<User></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseStringApp<T, TDbContext> :BaseApp<T,TDbContext> where T : StringEntity where TDbContext: DbContext
    {
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

        public T Get(string id)
        {
            return Repository.FirstOrDefault(u => u.Id == id);
        }

    }
}