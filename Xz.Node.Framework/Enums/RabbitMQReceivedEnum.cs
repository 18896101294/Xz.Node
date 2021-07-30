using System.ComponentModel;

namespace Xz.Node.Framework.Enums
{
    /// <summary>
    /// MQ消息确认枚举
    /// </summary>
    public enum RabbitMQReceivedEnum
    {
        /// <summary>
        /// 成功确认
        /// </summary>
        [Description("成功确认")]
        BasicAck = 1,
        /// <summary>
        /// 批量成功确认
        /// </summary>
        [Description("批量成功确认")]
        BasicAckMultiple = 2,
        /// <summary>
        /// 拒绝消息
        /// </summary>
        [Description("拒绝消息")]
        BasicReject = 3,
        /// <summary>
        /// 拒绝消息重入队列
        /// </summary>
        [Description("拒绝消息重入队列")]
        BasicRejectRequeue = 4,
        /// <summary>
        /// 否定应答
        /// </summary>
        [Description("否定应答")]
        BasicNack = 5,
        /// <summary>
        /// 否定应答重回队列
        /// </summary>
        [Description("否定应答重回队列")]
        BasicNackRequeue = 6
    }

}
