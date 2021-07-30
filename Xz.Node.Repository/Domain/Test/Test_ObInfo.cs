using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Test
{
    /// <summary>
    /// 测试表Test_Op关联表
    /// </summary>
    [Table("Test_Ob")]
    public class Test_ObInfo : GuidEntity
    {
        /// <summary>
        ///  测试表Test_Op关联表
        /// </summary>
        public Test_ObInfo()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
        }

        /// <summary>
        /// TestOp外键
        /// </summary>
        [Column("TestOpForeignKey")]
        [Description("TestOpForeignKey")]
        public Guid TestOpForeignKey { get; set; }
        /// <summary>
        /// 引用导航属性
        /// </summary>
        public Test_OpInfo TestOp { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [Column("Name")]
        [Description("应用名称")]
        public string Name { get; set; }
        /// <summary>
        /// 应用描述
        /// </summary>
        [Column("Description")]
        [Description("应用描述")]
        public string Description { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        [Column("Disable")]
        [Description("是否可用")]
        public bool Disable { get; set; }
    }
}
