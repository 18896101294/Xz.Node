using System;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace Xz.Node.Framework.Cache
{
    /// <summary>
    /// memcache缓存
    /// </summary>
    public sealed class EnyimMemcachedContext : ICacheContext
    {
        private  IMemcachedClient _memcachedClient;
        public EnyimMemcachedContext(IMemcachedClient client)
        {
            _memcachedClient = client;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override T Get<T>(string key)
        {
            return _memcachedClient.Get<T>(key);
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
            if(expire == null)
            {
                return _memcachedClient.Store(StoreMode.Set, key, t);
            }
            return _memcachedClient.Store(StoreMode.Set, key, t, expire.Value);
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool Remove(string key)
        {
            return _memcachedClient.Remove(key);
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool KeyExists(string key)
        {
            object value = null;
            return _memcachedClient.TryGet(key, out value);
        }
    }
}