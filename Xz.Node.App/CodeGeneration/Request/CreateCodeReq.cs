using System;
using System.Collections.Generic;
using System.Reflection;
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
        /// <summary>
        /// 表描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否创建业务层
        /// </summary>
        public bool IsApp { get; set; } = true;
        /// <summary>
        /// 是否创建控制器
        /// </summary>
        public bool IsController { get; set; }
        /// <summary>
        /// 是否创建前端
        /// </summary>
        public bool IsWeb { get; set; }
        /// <summary>
        /// 是否重载实体
        /// </summary>
        public bool IsModel { get; set; }
        /// <summary>
        /// 字段属性
        /// </summary>
        public IList<PropertyInfo> Properties { get; set; }
    }
}
