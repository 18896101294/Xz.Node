using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Test
{
    /// <summary>
    /// 测试表Test_On
    /// </summary>
    [Table("Test_On")]
    public class Test_OnInfo : GuidEntity
    {
        /// <summary>
        /// Test_On
        /// </summary>
        public Test_OnInfo()
        {

        }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Column("Name")]
        [Description("应用名称")]
        public string Name { get; set; }
        /// <summary>
        /// 集合导航属性
        /// </summary>
        public List<Test_On_OmInfo> Test_On_Oms { get; set; }
    }
}
