namespace Xz.Node.App.Auth.Role.Response
{
    /// <summary>
    /// 加载角色分页列表view
    /// </summary>
    public class LoadRolesPageView
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 状态 0：启用，1：禁用
        /// </summary>
        public int Status { get; set; }
    }
}
