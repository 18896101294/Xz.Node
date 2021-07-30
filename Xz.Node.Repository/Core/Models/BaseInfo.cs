using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Xz.Node.Repository.Core.Models
{
    /// <summary>
    /// 基础业务模型
    /// </summary>
    public class BaseInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseInfo()
        {
            this.CreateTime = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Creater = string.Empty;
            this.UpdateTime = DateTime.Now;
            this.Updater = string.Empty;
        }

        /// <summary>
        /// 是否物理删除
        /// </summary>
        [Column("is_delete"), Newtonsoft.Json.JsonIgnore]
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column("creater")]
        public string Creater { get; set; } = string.Empty;
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column("create_user_id")]
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新用户
        /// </summary>
        [Column("updater")]
        public string Updater { get; set; } = string.Empty;
        /// <summary>
        /// 更新用户
        /// </summary>
        [Column("update_user_id")]
        public Guid UpdateUserId { get; set; }
        /// <summary>
        /// 更新时间 
        /// </summary>
        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
