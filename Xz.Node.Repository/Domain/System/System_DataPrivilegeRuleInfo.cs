using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.System
{
    /// <summary>
	/// 系统授权规制表
	/// </summary>
    [Table("System_DataPrivilegeRule")]
    public partial class System_DataPrivilegeRuleInfo : GuidEntity
    {
        public System_DataPrivilegeRuleInfo()
        {
            this.SourceCode = string.Empty;
            this.SubSourceCode = string.Empty;
            this.Description = string.Empty;
            this.SortNo = 0;
            this.PrivilegeRules = string.Empty;
        }

        /// <summary>
        /// 资源标识（模块编号）
        /// </summary>
        [Description("模块编号")]
        public string SourceCode { get; set; }
        /// <summary>
        /// 二级资源标识
        /// </summary>
        [Description("二级资源标识")]
        [Browsable(false)]
        public string SubSourceCode { get; set; }
        /// <summary>
        /// 权限描述
        /// </summary>
        [Description("权限描述")]
        public string Description { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        [Description("排序号")]
        public int SortNo { get; set; }
        /// <summary>
        /// 权限规则
        /// </summary>
        [Description("权限规则")]
        public string PrivilegeRules { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        [Description("是否可用")]
        public bool Enable { get; set; }
    }
}
