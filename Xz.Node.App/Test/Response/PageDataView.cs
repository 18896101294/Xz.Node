using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xz.Node.App.Test.Response
{
    /// <summary>
    /// 测试分页列表返回View
    /// </summary>
    public class PageDataView
    {
        /// <summary>
        /// Id
        /// </summary>
        [Browsable(false)]
        [Description("ID")]
        public Guid Id { get; set; }
        /// <summary>
        /// 是否物理删除
        /// </summary>
        [Browsable(false)]
        [Description("是否物理删除")]
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Browsable(false)]
        [Description("创建用户")]
        public string Creater { get; set; } = string.Empty;
        /// <summary>
        /// 创建用户Id
        /// </summary>
        [Browsable(false)]
        [Description("创建用户Id")]
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新用户
        /// </summary>
        [Browsable(false)]
        [Description("更新用户")]
        public string Updater { get; set; } = string.Empty;
        /// <summary>
        /// 更新用户Id
        /// </summary>
        [Browsable(false)]
        [Description("更新用户Id")]
        public Guid UpdateUserId { get; set; }
        /// <summary>
        /// 更新时间 
        /// </summary>
        [Browsable(false)]
        [Description("更新时间")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        [Description("密钥")]
        public string AppSecret { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Description("图标")]
        public string Icon { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public bool Disable { get; set; }
    }
}
