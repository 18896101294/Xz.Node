using System;
using Microsoft.Extensions.Caching.Memory;

namespace Xz.Node.Framework.Cache
{
    /// <summary>
    /// 微软默认带超时的Cache
    /// .net的内存Cache，在用IIS发布后，由于IIS本身存在自动回收的机制，会导致系统缓存20分钟就会失效。
    /// </summary>
    public class CacheContext : ICacheContext
    {
        private readonly IMemoryCache _objCache;
        public CacheContext(IMemoryCache objCache)
        {
            _objCache = objCache;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override T Get<T>(string key)
        {
            return  _objCache.Get<T>(key);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="t">缓存对象</param>
        /// <param name="expire">缓存时间</param>
        /// <returns></returns>
        public override bool Set<T>(string key, T t, DateTime? expire)
        {
            var obj = Get<T>(key);
            if (obj != null)
            {
                Remove(key);
            }
            if (expire == null)
            {
                _objCache.Set(key, t);
            }
            else
            {
                _objCache.Set(key, t, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expire.Value));//绝对过期时间
            }
            return true;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool Remove(string key)
        {
            _objCache.Remove(key);
            return true;
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override bool KeyExists(string key)
        {
            object value = null;
            return _objCache.TryGetValue(key, out value);
        }
    }
}
