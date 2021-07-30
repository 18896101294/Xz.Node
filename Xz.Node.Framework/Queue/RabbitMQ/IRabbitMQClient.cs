using System.Collections.Generic;
using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Queue.RabbitMQ
{
    /// <summary>
    /// MQ 消息推送接口
    /// </summary>
    public interface IRabbitMQClient
    {
        /// <summary>
        /// 监听类型进行推送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        void PushMessage(MQListenererEnum type, object message);

        /// <summary>
        /// 指定队列进行推送
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        void PushMessage(string queue, string routingKey, object message, IDictionary<string, object> arguments);
    }
}
