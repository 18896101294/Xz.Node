using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.Framework.Queue.RabbitMQ
{
    public class RabbitMQClient : IRabbitMQClient
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;
        private readonly IConfiguration _configuration;
        public RabbitMQClient(ILogger<RabbitMQClient> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            try
            {
                _configuration = configuration;
                string hostName = _configuration["RabbitMQ:HostName"];//主机名
                int port = _configuration["RabbitMQ:Port"].ToInt();//端口 
                //int port =  AmqpTcpEndpoint.UseDefaultPort; 默认端口写法：默认端口为5672
                string userName = _configuration["RabbitMQ:UserName"];//用户名
                string password = _configuration["RabbitMQ:Password"];//密码
                string virtualHost = _configuration["RabbitMQ:VirtualHost"];//虚拟机

                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password,
                    Port = port,
                    VirtualHost = virtualHost
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("RabbitMQClient初始化失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 指定类型推送
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public virtual void PushMessage(MQListenererEnum type, object message)
        {
            var queue = string.Empty;
            var routingKey = string.Empty;
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            if (type == MQListenererEnum.Test)
            {
                queue = "xznodemq.test";
                routingKey = "xznoderk.test";
                arguments.Add("x-dead-letter-exchange", "xznode.message.dlx");
                arguments.Add("x-dead-letter-routing-key", "xznoderk.test.dlx");
            }
            //ttl
            arguments.Add("x-message-ttl", 1000 * 60);//message-ttl：单独指定message限定一个时间；queue TTL：限制队列中所有message限定一个时间。毫秒为单位，过期后队列消息消失
            PushMessage(queue, routingKey, message, arguments);
        }

        /// <summary>
        /// 指定队列推送
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        public virtual void PushMessage(string queue, string routingKey, object message, IDictionary<string, object> arguments = null)
        {
            //开启confirm模式
            _channel.ConfirmSelect();
            //声明队列
            _channel.QueueDeclare(queue: queue,
                                    durable: true,//是否持久化，true：表示持久化，会存盘，服务器重启仍然存在，false：非持久化
                                    exclusive: false,//是否排他，如果一个队列声明为排他队列，该队列公对首次声明它的连接可见，并在连接断开时自动删除
                                    autoDelete: false,//是否自动删除
                                    arguments: arguments);//扩展参数

            string msgJson = JsonHelper.Instance.Serialize(message);
            var body = Encoding.UTF8.GetBytes(msgJson);

            _channel.BasicPublish(exchange: "xznode.message",
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);
            //消息确认失败
            if (!_channel.WaitForConfirms())
            {
                _logger.LogError(string.Format("消息队列生产端消息确认失败,队列:{0},路由:{1},消息:{2}", queue, routingKey, message));
            }
        }
    }
}
