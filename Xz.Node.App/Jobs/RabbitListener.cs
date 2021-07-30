using Microsoft.Extensions.Logging;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.App.Jobs
{
    /// <summary>
    /// 消费者监听
    /// </summary>
    public class RabbitListener : IJob
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly ILogger<RabbitListener> _logger;
        /// <summary>
        /// 交换机
        /// </summary>
        protected string _exchange;
        /// <summary>
        /// 交换机类型
        /// </summary>
        protected string _type;
        /// <summary>
        /// 路由
        /// </summary>
        protected string _routeKey;
        /// <summary>
        /// 队列名称
        /// </summary>
        protected string _queueName;
        /// <summary>
        /// 一次接收消息数
        /// </summary>
        protected ushort _prefetchCount;
        /// <summary>
        /// 总数
        /// </summary>
        protected long _ttl;
        /// <summary>
        /// 死信队列交换机
        /// </summary>
        protected string _exchange_dlx;
        /// <summary>
        /// 死信队列路由键
        /// </summary>
        protected string _routeKey_dlx;
        /// <summary>
        /// 消费者监听
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exchange">交换机</param>
        /// <param name="type">交换机类型</param>
        /// <param name="routeKey">路由</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="prefetchCount">一次接收消息数</param>
        /// <param name="ttl">总数</param>
        /// <param name="exchange_dlx">死信队列交换机</param>
        /// <param name="routeKey_dlx">死信队列路由键</param>
        public RabbitListener(ILogger<RabbitListener> logger,
            string exchange, string type, string routeKey,
            string queueName, ushort prefetchCount, long ttl,
            string exchange_dlx, string routeKey_dlx)
        {
            _logger = logger;
            _exchange = exchange;
            _type = type;
            _routeKey = routeKey;
            _queueName = queueName;
            _prefetchCount = prefetchCount;
            _ttl = ttl;
            _exchange_dlx = exchange_dlx;
            _routeKey_dlx = routeKey_dlx;

            try
            {
                var configuration = ConfigHelper.GetConfigRoot();
                string hostName = configuration["RabbitMQ:HostName"];//主机名
                int port = configuration["RabbitMQ:Port"].ToInt();//端口
                //int port =  AmqpTcpEndpoint.UseDefaultPort; 默认端口写法：默认端口为5672
                string userName = configuration["RabbitMQ:UserName"];//用户名
                string password = configuration["RabbitMQ:Password"];//密码
                string virtualHost = configuration["RabbitMQ:VirtualHost"];//虚拟机

                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                    Port = port,
                    UserName = userName,
                    Password = password,
                    VirtualHost = virtualHost
                };
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("RabbitListener初始化失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            Register();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual int Process(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 注册消费者监听
        /// </summary>
        public void Register()
        {
            try
            {
                //声明交换机 exchange:
                channel.ExchangeDeclare(exchange: _exchange, type: _type, true);
                var arguments = new Dictionary<string, object>();
                if (_ttl > 0)
                {
                    arguments.Add("x-message-ttl", _ttl);
                }
                if (!string.IsNullOrEmpty(_exchange_dlx))
                {
                    arguments.Add("x-dead-letter-exchange", _exchange_dlx);
                }
                if (!string.IsNullOrEmpty(_routeKey_dlx))
                {
                    arguments.Add("x-dead-letter-routing-key", _routeKey_dlx);
                }
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                channel.QueueBind(queue: _queueName,
                                  exchange: _exchange,
                                  routingKey: _routeKey);
                var consumer = new EventingBasicConsumer(channel);
                //QoS = quality-of-service 服务质量
                //第一个参数是可接收消息的大小的，但是似乎在客户端2.8.6版本中它必须为0，即使：不受限制。如果不输0，程序会在运行到这一行的时候报错，说还没有实现不为0的情况。
                //第二个参数是处理消息最大的数量。举个例子，如果输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息，消息只会在队列中阻塞。如果输入3，那么可以最多有3个消息不应答，如果到达了3个，则发送端发给这个接收方得消息只会在队列中，而接收方不会有接收到消息的事件产生。总结说，就是在下一次发送应答消息前，客户端可以收到的消息最大数量。
                //第三个参数则设置了是不是针对整个Connection的，因为一个Connection可以有多个Channel，如果是false则说明只是针对于这个Channel的。
                channel.BasicQos(0, _prefetchCount, false);
                // 开启发送方确认模式
                channel.ConfirmSelect();
                consumer.Received += (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.Span);
                    int result = Process(message);
                    switch (result)
                    {
                        case (int)RabbitMQReceivedEnum.BasicAck:
                            channel.BasicAck(ea.DeliveryTag, false);
                            break;
                        case (int)RabbitMQReceivedEnum.BasicAckMultiple:
                            channel.BasicAck(ea.DeliveryTag, true);
                            break;
                        case (int)RabbitMQReceivedEnum.BasicReject:
                            channel.BasicReject(ea.DeliveryTag, false);
                            break;
                        case (int)RabbitMQReceivedEnum.BasicRejectRequeue:
                            channel.BasicReject(ea.DeliveryTag, true);
                            break;
                        case (int)RabbitMQReceivedEnum.BasicNack:
                            channel.BasicNack(ea.DeliveryTag, false, false);
                            break;
                        case (int)RabbitMQReceivedEnum.BasicNackRequeue:
                            channel.BasicNack(ea.DeliveryTag, false, true);
                            break;
                        default:
                            channel.BasicAck(ea.DeliveryTag, false);
                            break;
                    }
                };
                channel.BasicConsume(queue: _queueName, consumer: consumer);
            }
            catch (Exception ex)
            {
                this.connection.Close();
                _logger.LogError("RabbitListener注册消费者失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DeRegister()
        {
            this.connection.Close();
        }
    }
}
