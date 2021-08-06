using Elasticsearch.Net;
using Nest;

namespace Xz.Node.Framework.Elasticsearch
{
    /// <summary>
    /// ElasticSearch接口
    /// </summary>
    public interface IElasticSearchServer
    {
        /// <summary>
        /// Linq查询的官方Client
        /// </summary>
        IElasticClient ElasticLinqClient { get; set; }

        /// <summary>
        /// Json查询的官方Client
        /// </summary>
        IElasticLowLevelClient ElasticJsonClient { get; set; }
    }
}
