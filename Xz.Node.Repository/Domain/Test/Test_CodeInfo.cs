using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Test
{
    /// <summary>
    /// 测试类
    /// </summary>
    [Table("Test_Code")]
    [Description("测试类")]
    public class Test_CodeInfo : GuidEntity
    {
        /// <summary>
        /// 测试类
        /// </summary>
        public Test_CodeInfo()
        {
            this.Column2 = string.Empty;
            this.Column2 = string.Empty;
            this.Column2 = string.Empty;
        }

        /// <summary>
        /// 字段1
        /// </summary>
        [Column("Column1")]
        [Description("字段1")]
        public string Column1 { get; set; }
        /// <summary>
        /// 字段2
        /// </summary>
        [Column("Column2")]
        [Description("字段2")]
        public string Column2 { get; set; }
        /// <summary>
        /// 字段3
        /// </summary>
        [Column("Column3")]
        [Description("字段3")]
        public string Column3 { get; set; }
    }
}
