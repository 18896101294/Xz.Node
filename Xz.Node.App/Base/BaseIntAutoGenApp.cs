using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xz.Node.App.Interface;
using Xz.Node.Repository.Core;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// 数据库Id为numberic且为数据库自动生成的业务类型
    /// 该场景通常为SqlServer的自动增长类型和Oracle自带的Sequence
    /// 业务层基类，UnitWork用于事务操作，Repository用于普通的数据库操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public class BaseIntAutoGenApp<T, TDbContext> :BaseApp<T,TDbContext> where T : IntAutoGenEntity where TDbContext: DbContext
    {
        /// <summary>
        /// 数据库Id为numberic且为数据库自动生成的业务类型构造
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        public BaseIntAutoGenApp(IUnitWork<TDbContext> unitWork, IRepository<T,TDbContext> repository, IAuth auth) : base(unitWork, repository, auth)
        {
        }

        /// <summary>
        /// 按id批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void Delete(int[] ids)
        {
            Repository.Delete(u => ids.Contains(u.Id));
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            return Repository.FirstOrDefault(u => u.Id == id);
        }
    }
}