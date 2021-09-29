using System.ComponentModel;

namespace Xz.Node.App.System.Notice.Enums
{
    /// <summary>
    /// 通知类型枚举
    /// </summary>
    public enum NoticeTypeEnum
    {
        /// <summary>
        /// 系统通知
        /// </summary>
        [Description("系统通知")]
        BySystem = 1,
        /// <summary>
        /// 更新通知
        /// </summary>
        [Description("更新通知")]
        ByUpdate = 2
    }
}
