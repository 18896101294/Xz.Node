using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.App.SSO.Response
{
    /// <summary>
    /// 验证码返回view
    /// </summary>
    public class CaptchaViewModel
    {
        /// <summary>
        /// 图片字节流
        /// </summary>
        public byte[] ImgData { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
    }
}
