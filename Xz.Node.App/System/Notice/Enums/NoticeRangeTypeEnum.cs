using System.ComponentModel;

namespace Xz.Node.App.System.Notice.Enums
{
    /// <summary>
    /// 通知范围枚举
    /// </summary>
    public enum NoticeRangeTypeEnum
    {
        /// <summary>
        /// 通知所有人
        /// </summary>
        [Description("通知所有人")]
        All = 0,
        /// <summary>
        /// 按部门通知
        /// </summary>
        [Description("按部门通知")]
        ByOrgs = 1,
        /// <summary>
        /// 按角色通知
        /// </summary>
        [Description("按角色通知")]
        ByRoles = 2,
        /// <summary>
        /// 按用户通知
        /// </summary>
        [Description("按用户通知")]
        ByUsers = 3
    }
}
