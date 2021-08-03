using System;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Xz.Node.Framework.Common;

namespace Xz.Node.Framework.Cache
{
    /// <summary>
    /// 缓存redis实现
    /// </summary>
    public sealed class RedisCacheContext : ICacheContext
    {
        private ConnectionMultiplexer _conn { get; set; }
        private IDatabase iDatabase { get; set; }

        public RedisCacheContext()
        {
            _conn = ConnectionMultiplexer.Connect(ConfigHelper.GetConfigRoot()["AppSetting:RedisConf"]);
            iDatabase = _conn.GetDatabase();
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override T Get<T>(string key)
        {
            RedisValue value = iDatabase.StringGet(key);
            if (!value.HasValue)
            {
                return default(T);
            }
            
            if (typeof(T) == typeof(string))
            {
                return (T) Convert.ChangeType(value, typeof(T));
            }
            else
            {
                return JsonHelper.Instance.Deserialize<T>(value);
            }
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="t">缓存对象</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public override bool Set<T>(string key, T t, DateTime? expire)
        {
            if (typeof(T) == typeof(string))
            {
                if (expire == null)
                {
                    return iDatabase.StringSet(key, t.ToString());
                }
                return iDatabase.StringSet(key, t.ToString(), expire-DateTime.Now);
            }
            else
            {
                if(expire == null)
                {
                    return iDatabase.StringSet(key, JsonHelper.Instance.Serialize(t));
                }
                return iDatabase.StringSet(key, JsonHelper.Instance.Serialize(t), expire - DateTime.Now);
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool Remove(string key)
        {
            return iDatabase.KeyDelete(key);
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool KeyExists(string key)
        {
            return iDatabase.KeyExists(key);
        }
    }
}