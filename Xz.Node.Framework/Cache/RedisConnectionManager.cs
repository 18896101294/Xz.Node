using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Xz.Node.Framework.Cache
{
    /// <summary>
    /// ConnectionMultiplexer对象管理帮助类
    /// </summary>
    public static class RedisConnectionManager
    {
        /// <summary>
        /// Redis连接池
        /// </summary>
        private static readonly Dictionary<string, ConnectionMultiplexer> ConnectionCache = new Dictionary<string, ConnectionMultiplexer>();
        private static object _lockObj = new object();
        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>连接对象</returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(IConfiguration configuration)
        {
            var appId = configuration.GetSection("AppSetting")["AppId"];
            foreach (var key in ConnectionCache.Keys)
            {
                if (key.Contains(appId))
                {
                    return ConnectionCache[appId];
                }
            }

            lock (_lockObj)
            {
                var conn = GetManager(configuration);
                ConnectionCache.Add(appId, conn);
                return conn;
            }
        }

        /// <summary>
        /// 建立新连接
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static ConnectionMultiplexer GetManager(IConfiguration configuration)
        {
            if (configuration.GetSection("Redis:Sentinel").Exists())
            {
                //哨兵连接
                ConfigurationOptions sentinelOptions = new ConfigurationOptions();
                var sentinelArray = configuration.GetSection("Redis:Sentinel").GetChildren();
                foreach (var sentinel in sentinelArray)
                {
                    sentinelOptions.EndPoints.Add(sentinel.Value);
                }
                sentinelOptions.TieBreaker = "";
                sentinelOptions.CommandMap = CommandMap.Sentinel;
                sentinelOptions.AbortOnConnectFail = true;
                // Connect!
                var sentinelConnection = ConnectionMultiplexer.Connect(sentinelOptions);

                // Get a connection to the master
                var redisServiceOptions = new ConfigurationOptions();
                redisServiceOptions.ServiceName = configuration.GetSection("Redis:ServiceName").Value;   //master名称
                redisServiceOptions.Password = configuration.GetSection("Redis:Password").Value;     //master访问密码
                redisServiceOptions.AbortOnConnectFail = true;
                redisServiceOptions.AllowAdmin = true;
                var connect = sentinelConnection.GetSentinelMasterConnection(redisServiceOptions);
                return connect;
            }
            else
            {
                //单机连接
                string connectionString = configuration.GetSection("Redis:ConnectionStrings").Value;
                var connect = ConnectionMultiplexer.Connect(connectionString);
                return connect;
            }
        }

    }
}
