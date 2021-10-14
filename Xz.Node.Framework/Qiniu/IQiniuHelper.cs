using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xz.Node.Framework.Qiniu
{
    /// <summary>
    /// 七牛云帮助类接口
    /// </summary>
    public interface IQiniuHelper
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="category">所属模块</param>
        /// <returns></returns>
        List<string> UploadFile(IFormFileCollection files, string category);

        /// <summary>
        /// 获取七牛云秘钥、token等信息
        /// </summary>
        /// <returns></returns>
        (string, string, string, string) CreateUploadToken();
    }
}
