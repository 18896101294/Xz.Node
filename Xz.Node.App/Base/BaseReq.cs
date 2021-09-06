using System.Collections.Generic;

namespace Xz.Node.App.Base
{
    /// <summary>
    /// Id请求入参
    /// </summary>
    public class BaseIdReq
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Ids请求入参
    /// </summary>
    public class BaseIdsReq
    {
        /// <summary>
        /// Ids
        /// </summary>
        public List<string> Ids { get; set; }
    }
}
