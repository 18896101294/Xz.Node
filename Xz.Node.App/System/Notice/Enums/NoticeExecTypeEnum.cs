using System.ComponentModel;

namespace Xz.Node.App.System.Notice.Enums
{
    /// <summary>
    /// 执行方式枚举
    /// </summary>
    public enum NoticeExecTypeEnum
    {
        /// <summary>
        /// 立马执行
        /// </summary>
        [Description("立马执行")]
        RunNow = 1,
        /// <summary>
        /// 稍后执行
        /// </summary>
        [Description("稍后执行")]
        RunLater = 2
    }
}
