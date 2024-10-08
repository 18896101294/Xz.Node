﻿using System;

namespace Xz.Node.Framework.Model
{
    /// <summary>
    /// Consul服务实例
    /// </summary>
    public class ConsulServiceItemModel
    {
        /// <summary>
        /// 实例Id
        /// </summary>
        public Guid ServiceID { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 实例Ip地址
        /// </summary>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// 实例端口
        /// </summary>
        public int ServicePort { get; set; }

        /// <summary>
        /// 节点
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// Consul IP地址
        /// </summary>
        public string Address { get; set; }
    }
}
