using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.System.Consul.Response
{
    /// <summary>
    /// Consul配置
    /// </summary>
    public class ConsulSelfView
    {
        /// <summary>
        /// 基础配置
        /// </summary>
        public ConsulConfigModel Config { get; set; }
    }

    /// <summary>
    /// 基础配置
    /// </summary>
    public class ConsulConfigModel
    {
        /// <summary>
        /// 数据中心
        /// </summary>
        public string Datacenter { get;set;}

        /// <summary>
        /// 主数据中心
        /// </summary>
        public string PrimaryDatacenter { get; set; }

        /// <summary>
        /// 节点
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Server
        /// </summary>
        public bool Server { get; set; }
    }
}
