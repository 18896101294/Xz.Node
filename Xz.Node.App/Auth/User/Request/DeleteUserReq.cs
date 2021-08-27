using System.Collections.Generic;

namespace Xz.Node.App.Auth.User.Request
{
    /// <summary>
    /// 删除用户入参
    /// </summary>
    public class DeleteUserReq
    {
        /// <summary>
        /// 删除用户Id集合
        /// </summary>
        public List<string> Ids { get; set; }
    }
}
