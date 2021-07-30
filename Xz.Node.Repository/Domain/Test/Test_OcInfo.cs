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
    [Table("Test_Oc")]
    public class Test_OcInfo : GuidEntity
    {
        /// <summary>
        ///  测试表Test_Op关联表
        /// </summary>
        public Test_OcInfo()
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
        /// Test_Oc依赖实体，Test_OpInfo就是依赖主体
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
