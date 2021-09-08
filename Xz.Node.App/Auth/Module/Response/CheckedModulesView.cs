using System.Collections.Generic;
using Xz.Node.Repository.Domain.Auth;

namespace Xz.Node.App.Auth.Module.Response
{
    /// <summary>
    /// 获取勾选模块的信息View
    /// </summary>
    public class CheckedModulesView
    {
        /// <summary>
        /// 模块id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块路径名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 模块菜单列表
        /// </summary>
        public List<Auth_ModuleElementInfo> elements { get; set; }
    }
}
