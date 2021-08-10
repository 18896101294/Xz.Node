using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.System
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Table("System_Configuration")]
    public class System_ConfigurationInfo : GuidEntity
    {
        /// <summary>
        /// 构造
        /// </summary>
        public System_ConfigurationInfo()
        {
            this.Value = string.Empty;
            this.Text = string.Empty;
            this.DisplayNo = -1;
            this.Category = string.Empty;
            this.Remark = string.Empty;
        }
        /// <summary>
        /// 值
        /// </summary>
        [Column("Value")]
        [Description("值")]
        public string Value { get; set; }
        /// <summary>
        /// 显示值
        /// </summary>
        [Column("Text")]
        [Description("值显示值")]
        public string Text { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        [Column("DisplayNo")]
        [Description("顺序号")]
        public int DisplayNo { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [Column("Category")]
        [Description("分类")]
        public string Category { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("Remark")]
        [Description("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        [Column("IsHide")]
        [Description("是否隐藏")]
        public bool IsHide { get; set; }
    }
}
