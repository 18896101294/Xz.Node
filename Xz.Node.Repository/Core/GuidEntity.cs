using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Framework.Utilities;

namespace Xz.Node.Repository.Core
{
    /// <summary>
    /// 主键为Guid的实体基类，为系统默认的实体类型
    /// </summary>
    public class GuidEntity : BaseEntity
    {
        public GuidEntity()
        {
            this.CreateTime = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
            this.Creater = string.Empty;
            this.UpdateTime = DateTime.Now;
            this.Updater = string.Empty;
        }
        /// <summary>
        /// Id
        /// </summary>
        [Browsable(false)]
        [Column("Id")]
        [Description("Id")]
        public Guid Id { get; set; }
        /// <summary>
        /// 是否物理删除
        /// </summary>
        [Column("IsDelete"), Newtonsoft.Json.JsonIgnore]
        [Description("是否物理删除")]
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column("Creater")]
        [Description("创建用户")]
        public string Creater { get; set; } = string.Empty;
        /// <summary>
        /// 创建用户Id
        /// </summary>
        [Column("CreateUserId")]
        [Description("创建用户Id")]
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CreateTime")]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新用户
        /// </summary>
        [Column("Updater")]
        [Description("更新用户")]
        public string Updater { get; set; } = string.Empty;
        /// <summary>
        /// 更新用户Id
        /// </summary>
        [Column("UpdateUserId")]
        [Description("更新用户Id")]
        public Guid UpdateUserId { get; set; }
        /// <summary>
        /// 更新时间 
        /// </summary>
        [Column("UpdateTime")]
        [Description("更新时间")]
        public DateTime UpdateTime { get; set; }

        #region private
        /// <summary>
        /// 判断主键是否为空，常用做判定操作是【添加】还是【编辑】
        /// </summary>
        /// <returns></returns>
        public override bool KeyIsNull()
        {
            return Guid.Empty == Id ? true : false;
        }
        /// <summary>
        /// 创建默认的主键值
        /// </summary>
        public override void GenerateDefaultKeyVal()
        {
            Id = SequentialGuidGenerator.GenerateGuid();
        }
        #endregion
    }
}
