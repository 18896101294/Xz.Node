﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.System
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Table("System_OpenJob")]
    public partial class System_OpenJobInfo : StringEntity
    {
        public System_OpenJobInfo()
        {
            this.JobName = string.Empty;
            this.RunCount = 0;
            this.ErrorCount = 0;
            this.NextRunTime = DateTime.Now;
            this.LastRunTime = DateTime.Now;
            this.LastErrorTime = DateTime.Now;
            this.JobType = 0;
            this.JobCall = string.Empty;
            this.JobCallParams = string.Empty;
            this.Cron = string.Empty;
            this.Status = 0;
            this.Remark = string.Empty;
            this.CreateTime = DateTime.Now;
            this.CreateUserId = string.Empty;
            this.CreateUserName = string.Empty;
            this.UpdateTime = DateTime.Now;
            this.UpdateUserId = string.Empty;
            this.UpdateUserName = string.Empty;
            this.OrgId = string.Empty;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Description("任务名称")]
        public string JobName { get; set; }
        /// <summary>
        /// 任务执行次数
        /// </summary>
        [Description("任务执行次数")]
        public int RunCount { get; set; }
        /// <summary>
        /// 异常次数
        /// </summary>
        [Description("异常次数")]
        public int ErrorCount { get; set; }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        [Description("下次执行时间")]
        public DateTime? NextRunTime { get; set; }
        /// <summary>
        /// 最后一次执行时间
        /// </summary>
        [Description("最后一次执行时间")]
        public DateTime? LastRunTime { get; set; }
        /// <summary>
        /// 最后一次失败时间
        /// </summary>
        [Description("最后一次失败时间")]
        public DateTime? LastErrorTime { get; set; }
        /// <summary>
        /// 任务执行方式0：本地任务；1：外部接口任务
        /// </summary>
        [Description("任务执行方式0：本地任务；1：外部接口任务")]
        public int JobType { get; set; }
        /// <summary>
        /// 任务地址
        /// </summary>
        [Description("任务地址")]
        public string JobCall { get; set; }
        /// <summary>
        /// 任务参数，JSON格式
        /// </summary>
        [Description("任务参数，JSON格式")]
        public string JobCallParams { get; set; }
        /// <summary>
        /// CRON表达式
        /// </summary>
        [Description("CRON表达式")]
        public string Cron { get; set; }
        /// <summary>
        /// 任务运行状态（0：停止，1：正在运行，2：暂停）
        /// </summary>
        [Description("任务运行状态（0：停止，1：正在运行，2：暂停）")]
        public int Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        [Browsable(false)]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Description("最后更新时间")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 最后更新人ID
        /// </summary>
        [Description("最后更新人ID")]
        [Browsable(false)]
        public string UpdateUserId { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        [Description("最后更新人")]
        public string UpdateUserName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [Description("所属部门")]
        [Browsable(false)]
        public string OrgId { get; set; }
    }
}
