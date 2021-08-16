using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.Auth.Revelance.Request
{
    /// <summary>
    /// 比如给用户分配资源，那么firstId就是用户ID，secIds就是资源ID列表
    /// </summary>
    public class AssignReq
    {
        /// <summary>
        /// 分配的关键字，比如：UserRole
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// firstId就是用户ID
        /// </summary>
        public string firstId { get; set; }
        /// <summary>
        /// secIds就是角色ID列表
        /// </summary>
        public string[] secIds { get; set; }
    }
}
