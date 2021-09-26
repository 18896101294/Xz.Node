using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.User.Response
{
    /// <summary>
    /// 获取所有用户用于下拉框View
    /// </summary>
    public class LoadUserView
    {
        /// <summary>
        /// 分组名-部门
        /// </summary>
        public string Lable { get; set; }
        /// <summary>
        /// 用户数组
        /// </summary>
        public List<LoadUserModel> Options { get; set; }
    }

    /// <summary>
    /// 用户
    /// </summary>
    public class LoadUserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
    }
}
