using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.CodeGeneration.Request
{
    /// <summary>
    /// 代码生成入参
    /// </summary>
    public class CreateCodeReq
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; } = "Admin";

        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
    }
}
