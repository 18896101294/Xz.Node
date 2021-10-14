using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.AdminApi.Model;
using Xz.Node.App.System.Files.Request;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Framework.Qiniu;

namespace Xz.Node.AdminApi.Controllers.System.Files
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "文件管理")]
    public class FileController : ControllerBase
    {
        private readonly IQiniuHelper _qiniuHelper;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 文件管理，暂时只支持上传到七牛云
        /// </summary>
        /// <param name="qiniuHelper"></param>
        /// <param name="configuration"></param>
        public FileController(IQiniuHelper qiniuHelper,
            IConfiguration configuration)
        {
            _qiniuHelper = qiniuHelper;
            _configuration = configuration;
        }

        /// <summary>
        /// 上传文件到七牛云
        /// </summary>
        /// <returns></returns>
        [HttpPost, SwaggerFileUpload]
        public IActionResult UploadFileToQiNiu([FromQuery] UploadFileToQiNiuReq req)
        {
            var result = new ResultInfo<List<string>>()
            {
                Code = 200,
                Message = "上传成功",
            };
            var files = Request.Form.Files;
            if ((files?.Count ?? 0) <= 0)
            {
                throw new InfoException("不包含任何文件!");
            }
            var category = req.Category.ToLower();
            string[] categorys = (string[])_configuration.GetSection("Media:Categorys").Get(typeof(string[]));
            if (!categorys.Contains(category))
            {
                throw new InfoException($"不包含{ category }目录!");
            }

            var fileSize = _configuration.GetSection("Media:FileSize").ToInt();
            fileSize = fileSize == 0 ? 20 : fileSize;
            var limitSizes = files.Where(w => w.Length > (fileSize * 1024 * 1024));
            if (limitSizes?.Count() > 0)
            {
                throw new InfoException($"上传的文件中有超过{fileSize}M的文件");
            }

            string[] fileTypes = (string[])_configuration.GetSection("Media:FileTypes").Get(typeof(string[]));
            //获取文件后缀是否存在数组中
            var limitFiles = files.Where(w => !fileTypes.Contains(Path.GetExtension(w.FileName).ToLower()));

            if (limitFiles?.Count() > 0)
            {
                throw new InfoException("请上传指定格式的文件");
            }
            
            var fileList = _qiniuHelper.UploadFile(files, category);
            result.Data = fileList;

            return Ok(result);
        }

        /// <summary>
        /// 获取七牛云token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateUploadToken()
        {
            var (ak, sk, uploadToken, scope) = _qiniuHelper.CreateUploadToken();
            return Ok(new ResultInfo<object>()
            {
                Code = 200,
                Message = "获取成功",
                Data = new 
                {
                    Ak = ak,
                    Sk = sk,
                    UploadToken = uploadToken,
                    Scope = scope
                }
            });
        }
    }
}
