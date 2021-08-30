using Xz.Node.App.Base;

namespace Xz.Node.App.Auth.Role.Request
{
    /// <summary>
    /// 加载角色分页列表入参
    /// </summary>
    public class LoadRolesPageReq : PageReq
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 状态：0.启用，1.禁用
        /// </summary>
        public int? Status { get; set; }
    }
}
