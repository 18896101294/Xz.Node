using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.System
{
    /// <summary>
    /// 系统通知
    /// </summary>
    [Table("System_Notice")]
    public partial class System_NoticeInfo : GuidEntity
    {
        /// <summary>
        /// 系统通知
        /// </summary>
        public System_NoticeInfo()
        {
            this.Titile = string.Empty;
            this.Content = string.Empty;
            this.ExecTime = DateTime.Now;
            this.RangeIds = string.Empty;
            this.TenantId = string.Empty;
        }
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        public string Titile { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string Content { get; set; }
        /// <summary>
        /// 分类：1.系统通知，2.更新通知
        /// </summary>
        [Description("分类")]
        public int Type { get; set; }
        /// <summary>
        /// 执行类型 1.立马执行，2.稍后执行
        /// </summary>
        [Description("执行类型 1.立马执行，2.稍后执行")]
        public int ExecType { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        [Description("执行时间")]
        public DateTime ExecTime { get; set; }
        /// <summary>
        /// 通知范围：0.通知所有人，1.按部门通知，2.按角色通知，3.按用户通知
        /// </summary>
        [Description("通知范围：0.通知所有人，1.按部门通知，2.按角色通知，3.按用户通知")]
        public int RangeType { get; set; }
        /// <summary>
        /// 通知人 逗号拼接
        /// </summary>
        [Description("通知人 逗号拼接")]
        public string RangeIds { get; set; }
        /// <summary>
        /// 是否HTML通知样式 
        /// </summary>
        [Description("是否HTML通知样式 ")]
        public bool IsHtml { get; set; }
        /// <summary>
        /// 是否已执行
        /// </summary>
        [Description("是否已执行")]
        public bool IsExec { get; set; }
        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public string TenantId { get; set; }
    }
}
