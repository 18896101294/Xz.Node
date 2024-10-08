﻿using System;
using System.Collections.Generic;

namespace Xz.Node.App.System.Notice.Request
{
    /// <summary>
    /// 重新执行通知入参
    /// </summary>
    public class ReExecuteReq
    {
        /// <summary>
        /// 通知Id
        /// </summary>
        public List<Guid> Ids { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecTime { get; set; }
    }
}
