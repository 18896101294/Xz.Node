﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Auth
{
    /// <summary>
    /// 多对多关系集中映射
    /// </summary>
    [Table("Auth_Relevance")]
    public partial class Auth_RelevanceInfo : StringEntity
    {
        public Auth_RelevanceInfo()
        {
            this.Description = string.Empty;
            this.Key = string.Empty;
            this.Status = 0;
            this.OperateTime = DateTime.Now;
            this.OperatorId = string.Empty;
            this.FirstId = string.Empty;
            this.SecondId = string.Empty;
            this.ThirdId = string.Empty;
            this.ExtendInfo = string.Empty;
        }
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }
        /// <summary>
        /// 映射标识
        /// </summary>
        [Description("映射标识")]
        public string Key { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public int Status { get; set; }
        /// <summary>
        /// 授权时间
        /// </summary>
        [Description("授权时间")]
        public DateTime OperateTime { get; set; }
        /// <summary>
        /// 授权人
        /// </summary>
        [Description("授权人")]
        public string OperatorId { get; set; }
        /// <summary>
        /// 第一个表主键ID
        /// </summary>
        [Description("第一个表主键ID")]
        public string FirstId { get; set; }
        /// <summary>
        /// 第二个表主键ID
        /// </summary>
        [Description("第二个表主键ID")]
        public string SecondId { get; set; }
        /// <summary>
        /// 第三个主键
        /// </summary>
        [Description("第三个主键")]
        public string ThirdId { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        [Description("扩展信息")]
        public string ExtendInfo { get; set; }
    }
}
