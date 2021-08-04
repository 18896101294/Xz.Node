using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.App.Jobs
{
    /// <summary>
    /// MQ测试job，注意参数需要保持和rabbitMq的配置完全一致
    /// </summary>
    public class TestlListenerJob : RabbitListener
    {
        private readonly ILogger<TestlListenerJob> _logger;
        private readonly IConfiguration _configuration;
        private static string exchange = "xznode.message";
        private static string type = "topic";
        private static string routeKey = "xznoderk.test";
        private static string queueName = "xznodemq.test";
        private static ushort prefetchCount = 2;
        private static int ttl = 1000 * 60;//过期时间1分钟
        //声明死信队列
        private static string exchange_dlx = "xznode.message.dlx";//死信交换机
        private static string routeKey_dlx = "xznoderk.test.dlx";//死信路由
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public TestlListenerJob(ILogger<TestlListenerJob> logger, IConfiguration configuration) : base(logger, configuration, exchange, type, routeKey, queueName, prefetchCount, ttl, exchange_dlx, routeKey_dlx)
        {
            _logger = logger;
        }

        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override int Process(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return (int)RabbitMQReceivedEnum.BasicReject;
            };

            _logger.LogInformation("TestlListenerJob message:" + message);

            var id = message.ToInt();

            try
            {
                _logger.LogInformation(id + ",ExportExcelListener execute end");
                return (int)RabbitMQReceivedEnum.BasicAck;
            }
            catch (Exception ex)
            {
                _logger.LogError("任务中心监听事件异常,异常信息:" + ex.Message);
                return (int)RabbitMQReceivedEnum.BasicReject;
            }
        }
    }
}
