using System.Collections.Generic;
using Xz.Node.App.Interface;
using Xz.Node.App.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.App.AuthStrategies
{
    /// <summary>
    /// 授权策略上下文，一个典型的策略模式
    /// 根据用户账号的不同，采用不同的授权模式，以后可以扩展更多的授权方式
    /// </summary>
    public class AuthStrategyContext
    {
        private readonly IAuthStrategy _strategy;
        /// <summary>
        /// 授权策略
        /// </summary>
        /// <param name="strategy"></param>
        public AuthStrategyContext(IAuthStrategy strategy)
        {
            this._strategy = strategy;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public Auth_UserInfo User
        {
            get { return _strategy.User; }
        }

        /// <summary>
        /// 模块集合
        /// </summary>
        public List<ModuleView> Modules
        {
            get { return _strategy.Modules; }
        }

        /// <summary>
        /// 模块元素集合
        /// </summary>
        public List<Auth_ModuleElementInfo> ModuleElements
        {
            get { return _strategy.ModuleElements; }
        }

        /// <summary>
        /// 角色集合
        /// </summary>
        public List<Auth_RoleInfo> Roles
        {
            get { return _strategy.Roles; }
        }

        /// <summary>
        /// 资源表集合
        /// </summary>
        public List<Auth_ResourceInfo> Resources
        {
            get { return _strategy.Resources; }
        }

        /// <summary>
        /// 组织集合
        /// </summary>
        public List<Auth_OrgInfo> Orgs
        {
            get { return _strategy.Orgs; }
        }

        /// <summary>
        /// 可访问模块字段集合
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public List<KeyDescription> GetProperties(string moduleCode)
        {
            return _strategy.GetProperties(moduleCode);
        }
    }
}
