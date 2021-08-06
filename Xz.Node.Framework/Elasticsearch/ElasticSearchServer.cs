using Elasticsearch.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xz.Node.Framework.Elasticsearch
{
    /// <summary>
    /// ElasticSearch
    /// </summary>
    public class ElasticSearchServer : IElasticSearchServer
    {
        /// <summary>
        /// Linq查询的官方Client
        /// </summary>
        public IElasticClient ElasticLinqClient { get; set; }
        /// <summary>
        /// Json查询的官方Client
        /// </summary>
        public IElasticLowLevelClient ElasticJsonClient { get; set; }
        private static object _lockObj = new object();
        private static ConnectionSettings _settings = null;

        public ElasticSearchServer(IConfiguration configuration)
        {
            if (_settings != null)
            {
                this.ElasticJsonClient = new ElasticLowLevelClient(_settings);
                this.ElasticLinqClient = new ElasticClient(_settings);
                return;
            }
            lock (_lockObj)
            {
                //es的地址,支持连接池设置
                var uris = configuration.GetSection("ElasticSeach:Urls").GetChildren().Select(o => o.Value).ToList().ConvertAll(o => new Uri(o));
                var defaultIndex = configuration["ElasticSeach:DefaultIndex"];
                //配置es请求连接池
                var connectionPool = new StaticConnectionPool(uris);
                var settings = new ConnectionSettings(connectionPool)
                    //.BasicAuthentication("admin", "123456")//验证账号密码登录             
                    .RequestTimeout(TimeSpan.FromSeconds(30))//延迟 30s
                                                             //.DefaultIndex(defaultIndex)//默认索引名
                    ;
                //json查询的初始化
                this.ElasticJsonClient = new ElasticLowLevelClient(settings);
                //linq查询的初始化
                this.ElasticLinqClient = new ElasticClient(settings);
                _settings = settings;
            }
        }
    }
}
