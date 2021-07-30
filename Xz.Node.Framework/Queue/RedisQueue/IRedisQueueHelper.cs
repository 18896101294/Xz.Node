using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xz.Node.Framework.Queue.RedisQueue
{
    /// <summary>
    /// redis 队列接口
    /// </summary>
    public interface IRedisQueueHelper
    {
        /// <summary>
        /// 发布订阅
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="handler"></param>
        void SubScriper(string topticName, Action<RedisChannel, RedisValue> handler = null);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="message"></param>
        void PublishMessage(string topticName, string message);

        /// <summary>
        /// redis队列入队
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="redisvalue"></param>
        /// <returns></returns>
        Task<long> EnqueueListLeftPushAsync(RedisKey queueName, RedisValue redisvalue);

        /// <summary>
        /// redis队列出队
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        Task<string> DequeueListPopRightAsync(RedisKey queueName);

        /// <summary>
        /// 自增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long StringInc(string key);

        /// <summary>
        /// 自减
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long StringDec(string key);

        /// <summary>
        /// Redis分布式加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTimeSeconds"></param>
        void LockByRedis(string key, int expireTimeSeconds = 10);

        /// <summary>
        /// redis分布式解锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool UnLockByRedis(string key);
    }
}
