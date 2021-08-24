using System;
using Xz.Node.App.Base;

namespace Xz.Node.App.Request
{
    /// <summary>
    /// 系统日志请求参数
    /// </summary>
    public class QuerySysLogListReq : PageReq
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 操作Ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public int? Result { get; set; }

        /// <summary>
        /// 操作开始时间
        /// </summary>
        public DateTime? BeginCreateTime { get; set; }

        /// <summary>
        /// 操作结束时间
        /// </summary>
        public DateTime? EndCreateTime { get; set; }
    }
}