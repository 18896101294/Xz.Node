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
    [Table("Test_Op")]
    public class Test_OpInfo : GuidEntity
    {
        /// <summary>
        /// 测试类
        /// </summary>
        public Test_OpInfo()
        {
            this.Name = string.Empty;
            this.AppSecret = string.Empty;
            this.Description = string.Empty;
            this.Icon = string.Empty;
        }

        /// <summary>
        /// 应用名称
        /// </summary>
        [Column("Name")]
        [Description("应用名称")]
        public string Name { get; set; }
        /// <summary>
        /// 应用密钥
        /// </summary>
        [Column("AppSecret")]
        [Description("应用密钥")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 应用描述
        /// </summary>
        [Column("Description")]
        [Description("应用描述")]
        public string Description { get; set; }
        /// <summary>
        /// 应用图标
        /// </summary>
        [Column("Icon")]
        [Description("应用图标")]
        public string Icon { get; set; }
        /// <summary>
        /// Test_Oc依赖实体，Test_OpInfo就是依赖主体
        /// </summary>
        public Test_OcInfo TestOc { get; set; }
        /// <summary>
        /// Test_Oa依赖实体，Test_OpInfo就是依赖主体
        /// </summary>
        public Test_OaInfo TestOa { get; set; }
        /// <summary>
        /// 集合导航属性
        /// </summary>
        public List<Test_ObInfo> Test_Obs { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        [Column("Disable")]
        [Description("是否可用")]
        public bool Disable { get; set; }
    }
}
