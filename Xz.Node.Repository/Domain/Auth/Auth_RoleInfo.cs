﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Auth
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Table("Auth_Role")]
    public partial class Auth_RoleInfo : StringEntity
    {
        public Auth_RoleInfo()
        {
            this.Name = string.Empty;
            this.Status = 0;
            this.CreateTime = DateTime.Now;
            this.CreateId = string.Empty;
            this.TypeName = string.Empty;
            this.TypeId = string.Empty;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Description("角色名称")]
        public string Name { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        [Description("当前状态")]
        public int Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Description("创建人ID")]
        public string CreateId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [Description("分类名称")]
        public string TypeName { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        [Description("分类ID")]
        public string TypeId { get; set; }
        /// <summary>
        /// 角色代码
        /// </summary>
        [Description("角色代码")]
        public string Code { get; set; }
    }
}
