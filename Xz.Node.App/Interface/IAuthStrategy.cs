using System.Collections.Generic;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.App.Interface
{
    /// <summary>
    /// 授权策略接口
    /// </summary>
    public interface IAuthStrategy
    {
        List<ModuleView> Modules { get; }

        List<Auth_ModuleElementInfo> ModuleElements { get; }

        List<Auth_RoleInfo> Roles { get; }

        List<Auth_ResourceInfo> Resources { get; }

        List<Auth_OrgInfo> Orgs { get; }

        Auth_UserInfo User { get; set; }

        /// <summary>
        /// 根据模块id获取可访问的模块字段
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        List<KeyDescription> GetProperties(string moduleCode);
    }
}