using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.Framework.Model
{
    /// <summary>
    /// 系统表
    /// </summary>
    public class SysTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 表描述
        /// </summary>
        public string TableDescription { get; set; }
    }
}
