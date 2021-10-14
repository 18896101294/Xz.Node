using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.Framework.Qiniu
{
    /// <summary>
    /// 七牛云帮助类
    /// </summary>
    public class QiniuHelper : IQiniuHelper
    {
        private readonly IConfiguration _configuration;
        private readonly static UploadManager um = new UploadManager();
        public QiniuHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<string> UploadFile(IFormFileCollection files, string category)
        {
            var (ak, sk, uploadToken, scope) = this.CreateUploadToken();

            //false 使用https 自动识别存储空间            
            Config.AutoZone(ak, scope, false);

            var fileUrls = new List<string>();
            foreach (IFormFile file in files)//获取多个文件列表集合
            {
                if (file.Length > 0)
                {
                    Stream stream = file.OpenReadStream();
                    //var fileName = ContentDispositionHeaderValue
                    //.Parse(file.ContentDisposition)
                    //.FileName
                    //.Trim('"');
                    string fileName = file.FileName.Substring(file.FileName.LastIndexOf('.')); //文件扩展名
                    //DateTime.Now.ToString("yyyyMMddHHmmssffffff")
                    var saveKey = $"/{category}/" + Guid.NewGuid().ToString("N") + fileName;//重命名文件加上时间戳 其中上传地址也可以配置s   
                    HttpResult result = um.UploadStream(stream, saveKey, uploadToken);

                    if (result.Code == 200)
                    {
                        var resultModel = JsonHelper.Instance.Deserialize<QiniuResultTextModel>(result.Text);
                        fileUrls.Add(resultModel.Key);
                    }
                    throw new Exception(result.RefText);//上传失败错误信息
                }
            }
            return fileUrls;
        }

        /// <summary>
        /// 获取七牛云秘钥、token等信息
        /// </summary>
        /// <returns></returns>
        public (string, string, string, string) CreateUploadToken()
        {
            var ak = string.Empty;
            var sk = string.Empty;
            var scope = _configuration.GetSection("QiniuConfig:Scope").Value;
            var expires = _configuration.GetSection("QiniuConfig:Expires").Value.ToInt();
            //根据七牛云的文档指示，可以交替使用两对秘钥以增加安全性
            var enabledKey = _configuration.GetSection("QiniuConfig:EnabledKey").Value.ToInt();
            if (enabledKey == 1)
            {
                //第一对秘钥
                ak = _configuration.GetSection("QiniuConfig:AccessKey").Value;
                sk = _configuration.GetSection("QiniuConfig:SecretKey").Value;
            }
            else
            {
                //第二对秘钥
                ak = _configuration.GetSection("QiniuConfig:AccessKey1").Value;
                sk = _configuration.GetSection("QiniuConfig:SecretKey1").Value;
            }
            var mac = new Mac(ak, sk);
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy

            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;  
            //上传储存的空间名称
            putPolicy.Scope = scope;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(expires);

            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            // putPolicy.DeleteAfterDays = 1;
            string jstr = putPolicy.ToJsonString();
            //获取上传凭证
            var uploadToken = Auth.CreateUploadToken(mac, jstr);

            return (ak, sk, uploadToken, scope);
        }
    }

    /// <summary>
    /// 七牛云上传result解析
    /// </summary>
    public class QiniuResultTextModel
    {
        /// <summary>
        /// Hash码
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// key值（其实就是文件名，对应的就是我们的上传相对路径）
        /// </summary>
        public string Key { get; set; }
    }
}
