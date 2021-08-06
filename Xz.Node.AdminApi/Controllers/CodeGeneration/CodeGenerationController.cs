using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.CodeGenerationManager;
using Xz.Node.App.CodeGenerationManager.Request;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers.CodeGeneration
{
    /// <summary>
    /// 代码生成
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "代码生成")]
    public class CodeGenerationController : Controller
    {
        private readonly CodeGenerationApp _app;
        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="app"></param>
        public CodeGenerationController(CodeGenerationApp app)
        {
            _app = app;
        }

        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        [HttpGet]
        public IActionResult GetTables()
        {
            var result = new ResultInfo<IList<SysTable>>()
            {
                Message = "获取成功",
            };

            var resultData = _app.GetTables();
            result.Data = resultData;

            return Ok(result);
        }

        /// <summary>
        /// 根据数据库表名获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDbTableStructure(string tableName)
        {
            var result = new ResultInfo<IList<SysTableColumn>>()
            {
                Message = "获取成功",
            };
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new InfoException("数据库表名不能为空");
            }

            var resultData = _app.GetDbTableStructure(tableName);
            result.Data = resultData;

            return Ok(result);
        }

        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateCode(CreateCodeReq req)
        {
            var result = new ResultInfo<object>()
            {
                Message = "生成成功",
            };
            if (string.IsNullOrWhiteSpace(req.TableName))
            {
                throw new InfoException("数据库表名不能为空");
            }
            if (string.IsNullOrWhiteSpace(req.NameSpace))
            {
                throw new InfoException("命名空间不能为空");
            }
            _app.CreateCode(req);
            return Ok(result);
        }
    }
}
