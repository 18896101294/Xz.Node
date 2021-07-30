using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Test
{
    /// <summary>
    /// 测试表Test_On,Test_Om关联表
    /// </summary>
    [Table("Test_On_Om")]
    public class Test_On_OmInfo : GuidEntity
    {
        /// <summary>
        ///  测试表Test_Op关联表
        /// </summary>
        public Test_On_OmInfo()
        {
           
        }

        /// <summary>
        /// TestOn主键
        /// </summary>
        [Column("TestOnKey")]
        [Description("TestOn主键")]
        public Guid TestOnKey { get; set; }
        /// <summary>
        /// Test_On
        /// </summary>
        public Test_OnInfo TestOn { get; set; }
        /// <summary>
        /// TestOm主键
        /// </summary>
        [Column("TestOmKey")]
        [Description("TestOm主键")]
        public Guid TestOmKey { get; set; }
        /// <summary>
        /// Test_Om
        /// </summary>
        public Test_OmInfo TestOm { get; set; }
    }
}
