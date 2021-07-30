using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.UserManager
{
    public class UserManagerApp : BaseStringApp<Auth_UserInfo, XzDbContext>
    {
        public UserManagerApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_UserInfo, XzDbContext> repository,
            IAuth auth) : base(unitWork, repository, auth)
        {

        }

        /// <summary>
        /// 根据登录账号获取登录信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Auth_UserInfo GetByAccount(string account)
        {
            return Repository.FirstOrDefault(u => u.Account == account);
        }
    }
}
