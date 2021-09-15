namespace Xz.Node.App.Auth.Module.Response
{
    /// <summary>
    /// 获取角色目前拥有的数据字段view
    /// </summary>
    public class LoadPropertiesForRoleView
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// 数据字段id
        /// </summary>
        public string KeyId { get; set; }
    }
}
