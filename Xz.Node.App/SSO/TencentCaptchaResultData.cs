using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// 腾讯防水墙验证数据解析
    /// </summary>
    public class TencentCaptchaResultData
    {
        /// <summary>
        /// 验证码：1.验证成功，0.验证失败，100.AppSecretKey参数校验错误[required]
        /// </summary>
        public int response { get; set; }

        /// <summary>
        /// [0,100]，恶意等级
        /// </summary>
        public int evil_level { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string err_msg { get; set; }
    }
}
