using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xz.Node.Framework.Common;

namespace Xz.Node.Framework.Queue.RedisQueue
{
    /// <summary>
    /// redis 实现队列帮助类（简单的队列的时候可以使用redis作为mq，list数据类型比较适合）
    /// </summary>
    public class RedisQueueHelper : IRedisQueueHelper
    {
        private ConnectionMultiplexer _conn { get; set; }
        private ISubscriber _subscriber { get; set; }//订阅者
        private IDatabase _database { get; set; }
        public RedisQueueHelper(IOptions<AppSetting> options)
        {
            _conn = ConnectionMultiplexer.Connect(options.Value.RedisConf);
            _subscriber = _conn.GetSubscriber();
            _database = _conn.GetDatabase();
        }

        #region 发布订阅
        public void SubScriper(string topticName, Action<RedisChannel, RedisValue> handler = null)
        {
            ChannelMessageQueue channelMessageQueue = _subscriber.Subscribe(topticName);
            channelMessageQueue.OnMessage(channelMessage =>
            {
                if (handler != null)
                {
                    string redisChannel = channelMessage.Channel;
                    string msg = channelMessage.Message;
                    handler.Invoke(redisChannel, msg);
                }
                else
                {
                    string msg = channelMessage.Message;
                    Console.WriteLine($"订阅到消息: { msg},Channel={channelMessage.Channel}");
                }
            });
        }
        public void PublishMessage(string topticName, string message)
        {
            long publishLong = _subscriber.Publish(topticName, message);
            Console.WriteLine($"发布消息成功：{publishLong}");
        }

        #endregion

        #region 入队出队
        /// <summary>
        /// redis队列入队
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="redisvalue"></param>
        /// <returns></returns>
        public async Task<long> EnqueueListLeftPushAsync(RedisKey queueName, RedisValue redisvalue)
        {
            return await _database.ListLeftPushAsync(queueName, redisvalue);
        }

        /// <summary>
        /// redis队列出队，没获取一次会自动消费掉一条消息
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task<string> DequeueListPopRightAsync(RedisKey queueName)
        {
            int count = (await _database.ListRangeAsync(queueName)).Length;
            if (count <= 0)
            {
                throw new Exception($"队列{queueName}数据为零");
            }
            string redisValue = await _database.ListRightPopAsync(queueName);
            if (!string.IsNullOrEmpty(redisValue))
                return redisValue;
            else
                return string.Empty;
        }

        /// <summary>
        /// 值自增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringInc(string key)
        {
            return _database.StringIncrement(key);
        }

        /// <summary>
        /// 值自减
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringDec(string key)
        {
            return _database.StringDecrement(key);
        }
        #endregion

        #region 分布式锁
        /// <summary>
        /// Redis加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTimeSeconds"></param>
        public void LockByRedis(string key, int expireTimeSeconds = 10)
        {
            try
            {
                while (true)
                {
                    expireTimeSeconds = expireTimeSeconds > 20 ? 10 : expireTimeSeconds;
                    bool lockflag = _database.LockTake(key, Thread.CurrentThread.ManagedThreadId, TimeSpan.FromSeconds(expireTimeSeconds));
                    if (lockflag)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis加锁异常:{ex.Message}");
            }
        }

        /// <summary>
        /// redis解锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool UnLockByRedis(string key)
        {
            try
            {
                return _database.LockRelease(key, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis解锁异常:{ex.Message}");
            }
        }
        #endregion
    }
}
