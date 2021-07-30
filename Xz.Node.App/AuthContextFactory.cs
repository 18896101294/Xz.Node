using Xz.Node.App.AuthStrategies;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App
{
    /// <summary>
    /// 用户权限策略工厂,加载用户所有可访问的资源/机构/模块
    /// </summary>
    public class AuthContextFactory
    {
        private readonly SystemAuthStrategy _systemAuth;
        private readonly NormalAuthStrategy _normalAuthStrategy;
        private readonly IUnitWork<XzDbContext> _unitWork;
        /// <summary>
        /// 用户权限策略工厂,加载用户所有可访问的资源/机构/模块
        /// </summary>
        /// <param name="sysStrategy"></param>
        /// <param name="normalAuthStrategy"></param>
        /// <param name="unitWork"></param>
        public AuthContextFactory(SystemAuthStrategy sysStrategy
            , NormalAuthStrategy normalAuthStrategy
            , IUnitWork<XzDbContext> unitWork)
        {
            _systemAuth = sysStrategy;
            _normalAuthStrategy = normalAuthStrategy;
            _unitWork = unitWork;
        }

        /// <summary>
        /// 加载用户所有可访问的资源/机构/模块
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AuthStrategyContext GetAuthStrategyContext(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;
            IAuthStrategy service = null;
            if (username == Define.SYSTEM_USERNAME)
            {
                service = _systemAuth;
            }
            else
            {
                service = _normalAuthStrategy;
                service.User = _unitWork.FirstOrDefault<Auth_UserInfo>(u => u.Account == username);
            }
            return new AuthStrategyContext(service);
        }
    }
}