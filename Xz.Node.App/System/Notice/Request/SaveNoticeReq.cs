using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.System.Notice.Request
{
    /// <summary>
    /// 添加系统通知入参
    /// </summary>
    public class SaveNoticeReq
    {
        /// <summary>
        /// 通知Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Titile { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 分类：1.系统通知，2.更新通知
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 执行类型 1.立马执行，2.稍后执行
        /// </summary>
        public int ExecType { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecTime { get; set; }
        /// <summary>
        /// 通知范围：0.通知所有人，1.按部门通知，2.按角色通知，3.按用户通知
        /// </summary>
        public int RangeType { get; set; }
        /// <summary>
        /// 通知人Ids
        /// </summary>
        public List<string> RangeIds { get; set; }
        /// <summary>
        /// 是否HTML通知样式 
        /// </summary>
        public bool IsHtml { get; set; }
        /// <summary>
        /// 状态 0.启用，1.禁用
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 租户Id
        /// </summary>
        public string TenantId { get; set; }
    }
}
