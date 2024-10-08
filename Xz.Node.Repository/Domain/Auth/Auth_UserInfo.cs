﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xz.Node.Repository.Core;

namespace Xz.Node.Repository.Domain.Auth
{
    /// <summary>
    /// 用户基本信息表
    /// </summary>
    [Table("Auth_User")]
    public partial class Auth_UserInfo : StringEntity
    {
        public Auth_UserInfo()
        {
            this.Account = string.Empty;
            this.Password = string.Empty;
            this.Name = string.Empty;
            this.Sex = 0;
            this.Status = 0;
            this.BizCode = string.Empty;
            this.CreateTime = DateTime.Now;
            this.CreateId = string.Empty;
            this.TypeName = string.Empty;
            this.TypeId = string.Empty;
            this.Avatar = string.Empty;
        }

        /// <summary>
        /// 用户登录帐号
        /// </summary>
        [Description("用户登录帐号")]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string Password { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [Description("用户姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public int Sex { get; set; }
        /// <summary>
        /// 用户状态，0：启用，1：禁用
        /// </summary>
        [Description("用户状态，0：启用，1：禁用")]
        public int Status { get; set; }
        /// <summary>
        /// 业务对照码
        /// </summary>
        [Description("业务对照码")]
        public string BizCode { get; set; }
        /// <summary>
        /// 经办时间
        /// </summary>
        [Description("经办时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
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
        /// 头像
        /// </summary>
        [Description("头像")]
        public string Avatar { get; set; }
    }
}