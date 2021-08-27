namespace Xz.Node.App.Auth.User.Request
{
    /// <summary>
    /// 修改用户基本信息入参
    /// </summary>
    public class ChangeProfileReq
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string BizCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 用户状态，0：启用，1：禁用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
    }
}
