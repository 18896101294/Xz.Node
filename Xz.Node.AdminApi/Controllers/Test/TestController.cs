using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Xz.Node.App.Test;
using Xz.Node.App.Test.Response;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository.Domain.Test;
using Microsoft.AspNetCore.Authorization;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Queue.RabbitMQ;
using Xz.Node.Framework.Enums;
using System.Threading;
using Microsoft.Extensions.Options;
using Consul;
using System.Net.Http;
using System.Threading.Tasks;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace Xz.Node.AdminApi.Controllers.Test
{
    /// <summary>
    /// 单元测试
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "单元测试")]
    public class TestController : ControllerBase
    {
        private readonly TestOpApp _app;
        private readonly IRabbitMQClient _rabbitMQClient;
        private readonly IConfiguration _configuration;
        private readonly ICacheContext _cacheContext;
        private readonly IElasticSearchServer _elasticSearchServer;
        /// <summary>
        /// 单元测试
        /// </summary>
        /// <param name="app"></param>
        /// <param name="rabbitMQClient"></param>
        /// <param name="cacheContext"></param>
        /// <param name="configuration"></param>
        /// <param name="elasticSearchServer"></param>
        public TestController(TestOpApp app,
            IRabbitMQClient rabbitMQClient,
            ICacheContext cacheContext,
            IConfiguration configuration,
            IElasticSearchServer elasticSearchServer)
        {
            _app = app;
            _rabbitMQClient = rabbitMQClient;
            _cacheContext = cacheContext;
            _configuration = configuration;
            _elasticSearchServer = elasticSearchServer;
        }

        #region 单表操作
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(Test_OpInfo data)
        {
            var result = new ResultInfo<Test_OpInfo>()
            {
                Message = "保存成功",
                Data = data
            };

            if (data.KeyIsNull())
            {
                _app.Insert(data);
            }
            else
            {
                _app.Update(data);
            }
            return Ok(result);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BatchEdit(string name)
        {
            var result = new ResultInfo<object>()
            {
                Message = "保存成功",
            };
            var datas = _app.FindByName(name);
            foreach (var data in datas)
            {
                data.Name = "嘿嘿";
            }
            _app.Update(datas);
            return Ok(result);
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(IList<Guid> ids)
        {
            var result = new ResultInfo<object>()
            {
                Message = "删除成功",
            };
            _app.Delete(ids);
            return Ok(result);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageData([FromBody] BaseDto.PageDataModel dto)
        {
            //throw new InfoException("我是一个自定义异常");

            var result = new ResultInfo<PageInfo<Test_OpInfo>>()
            {
                Message = "获取数据成功",
            };

            result.Data = _app.GetPageDatas(dto.Conditions.ToConditions(), dto.Sorts.ToSorts(), dto.PageIndex ?? 1, dto.PageSize ?? 20);

            return Ok(result);
        }
        #endregion

        #region 多表操作
        /// <summary>
        /// 一对一两表关联查询，Lambda表达式写法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TestOneToOneLambdaFun()
        {
            var result = new ResultInfo<dynamic>()
            {
                Message = "获取成功"
            };
            result.Data = _app.TestOneToOneLambdaFun();
            return Ok(result);
        }

        /// <summary>
        /// 一对一两表关联查询，Linq表达式写法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TestOneToOneLinqFun()
        {
            var result = new ResultInfo<List<TestOneToOneLinqFunVM>>()
            {
                Message = "获取成功",
                Data = new List<TestOneToOneLinqFunVM>()
            };
            result.Data = _app.TestOneToOneLinqFun().ToList();
            return Ok(result);
        }

        /// <summary>
        /// 一对多两表关联查询，Lambda表达式写法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TestOneToMoreLambdaFun()
        {
            var result = new ResultInfo<dynamic>()
            {
                Message = "获取成功"
            };
            result.Data = _app.TestOneToMoreLambdaFun();
            return Ok(result);
        }

        /// <summary>
        /// 一对多两表关联查询，Linq表达式写法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TestOneToMoreLinqFun()
        {
            var result = new ResultInfo<dynamic>()
            {
                Message = "获取成功"
            };
            result.Data = _app.TestOneToMoreLinqFun();
            return Ok(result);
        }

        /// <summary>
        /// 单表分组的写法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OneTableGroupFun()
        {
            var result = new ResultInfo<dynamic>()
            {
                Message = "获取成功"
            };
            result.Data = _app.OneTableGroupFun();
            return Ok(result);
        }
        #endregion

        #region 导入导出
        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DownloadTemplate(string fileName)
        {
            var excel = _app.GetImportTemplate();
            return File(excel, "application/vnd.ms-excel", $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss") }.xlsx");
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual IActionResult Export([FromQuery] BaseDto.ExportModel model)
        {
            var steam = _app.Export(model.Conditions.ToConditions(), model.Sorts.ToSorts());
            return File(steam, "application/vnd.ms-excel", $"{model.FileName}_{DateTime.Now.ToString("yyyyMMddHHmmss") }.xlsx");
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult Import(IFormFile formFile)
        {
            var result = new ResultInfo<object>()
            {
                Message = "导入数据成功!"
            };

            if (Request.Form.Files.Count <= 0)
            {
                throw new InfoException("请上传需要导入的文件");
            }
            using (var stream = Request.Form.Files[0].OpenReadStream())
            {
                _app.Import(stream);
            }

            return Ok(result);
        }
        #endregion

        #region Ocelot
        /// <summary>
        /// Ocelot get请求
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult OcelotGetFun(int time)
        {
            var result = new ResultInfo<string>()
            {
                Message = "调用成功",
            };
            Thread.Sleep(time);
            result.Data = $"Get, This is from {HttpContext.Request.Host.Value}, path: {HttpContext.Request.Path}";
            return Ok(result);
        }

        /// <summary>
        /// Ocelot post请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult OcelotPostFun()
        {
            var result = new ResultInfo<string>()
            {
                Message = "调用成功",
            };
            result.Data = $"Post, This is from {HttpContext.Request.Host.Value}, path: {HttpContext.Request.Path}";
            return Ok(result);
        }
        #endregion

        #region Consul
        /// <summary>
        /// 模拟Consul请求
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConsulGetFun()
        {
            var result = new ResultInfo<string>()
            {
                Message = "调用成功",
            };

            string consulAddress = _configuration["Consul:ConsulAddress"];
            using var consulClient = new ConsulClient(a => a.Address = new Uri(consulAddress));
            var catalogServices = await consulClient.Catalog.Service("AdminApi");//这里是根据服务名获取注册的服务
            var services = catalogServices.Response;
            if (services != null && services.Any())
            {
                // 模拟随机一台进行请求，这里只是测试，可以选择合适的负载均衡框架
                var r = new Random();
                int index = r.Next(services.Count());
                var service = services.ElementAt(index);
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://{service.ServiceAddress}:{service.ServicePort}/api/Test/ConsulTest");
                string httpResult = await response.Content.ReadAsStringAsync();
                result.Message = httpResult;
            }
            else
            {
                result.Message = "未找到服务";
            }

            return Ok(result);
        }

        /// <summary>
        /// Consul测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public string ConsulTest()
        {
            return "嘿嘿";
        }

        #endregion

        #region Redis
        /// <summary>
        /// Redis测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedisTest()
        {
            _cacheContext.Set<string>("TestKey", "嘿嘿", null);

            return Ok();
        }
        #endregion

        #region RabbitMQ
        /// <summary>
        /// MQ测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult MqTest()
        {
            _rabbitMQClient.PushMessage(MQListenererEnum.Test, "这是一条测试消息");
            return Ok();
        }
        #endregion

        #region ElasticSearch
        /// <summary>
        /// 基于Linq的查询
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<Persons>> SearchByLinq(string user = "批量")
        {
            var list = await _elasticSearchServer.SearchByLinqAsync<Persons>("users", op => op.Match(ss => ss.Field(qq => qq.user == user)));

            return list;
        }

        /// <summary>
        /// 基于Json的查询
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<string> SearchByJson(string user = "批量")
        {
            var jsonobject = new { query = new { match = new { user = "批量" } } };
            string json = JsonHelper.Instance.Serialize(jsonobject);

            var jToken = await _elasticSearchServer.SearchByJsonAsync("users", json);

            return jToken.ToString();
        }

        /// <summary>
        /// 插入单个文档
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> InsertDocument()
        {
            var content = new
            {
                user = "嘻嘻",
                post_date = "2021-10-11T15:00:12",
                message = "...."
            };

            bool result = await _elasticSearchServer.InsertDocumentAsync("users", content, "1");
            return result;
        }

        /// <summary>
        /// 删除单个文档
        /// </summary>
        /// <param name="index"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> DeleteDocument(string index = "users", string id = "2")
        {
            bool result = await _elasticSearchServer.DeleteDocumentAsync(index, id);
            return result;
        }

        /// <summary>
        /// 通过Bulk更新文档  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> UpdateDocumentByBulk()
        {
            var content = new
            {
                user = "嘻嘻2",
                post_date = "2021-10-11T15:00:12",
                message = "更新一下"
            };

            bool result = await _elasticSearchServer.UpdateDocumentByBulkAsync("users", "1", content);
            return result;
        }

        /// <summary>
        /// 更新文档  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> UpdateDocument()
        {
            var content = new
            {
                user = "嘻嘻3",
                post_date = "2021-10-11T15:00:12",
                message = "更新一下333"
            };

            bool result = await _elasticSearchServer.UpdateDocumentAsync("users", "1", content);
            return result;
        }

        /// <summary>
        /// 批量更新文档
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> BatchUpdateDocumentByBulk()
        {
            List<object> contens = new List<object>()
            {
                new
                {
                    id = 1,
                    user = "嘻嘻_批量更新",
                    post_date = "2021-10-11T15:00:12",
                    message = "批量更新_id_1"
                },
                new
                {
                    id = 2,
                    user = "xxx_批量更新",
                    post_date = "2021-10-11T15:00:12",
                    message = "批量更新_id_2"
                }
            };
            bool result = await _elasticSearchServer.BatchUpdateDocumentByBulkAsync("users", contens);
            return result;
        }

        /// <summary>
        /// 通过索引与id检查文档是否已经存在
        /// </summary>
        /// <param name="index"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> SourceExistsAsync(string index = "users", string id = "1")
        {
            bool result = await _elasticSearchServer.SourceExistsAsync(index, id);
            return result;
        }

        /// <summary>
        /// 检测索引是否已经存在
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> IsIndexExsit(string index = "users")
        {
            bool result = await _elasticSearchServer.IsIndexExsit(index);
            return result;
        }

        /// <summary>
        /// 创建index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> CreateIndex(string index)
        {
            if (string.IsNullOrWhiteSpace(index))
                return false;

            bool result = await _elasticSearchServer.CreateIndexAsync(index);
            return result;
        }

        /// <summary>
        /// 删除index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> DeleteIndex(string index)
        {
            if (string.IsNullOrWhiteSpace(index))
                return false;

            bool result = await _elasticSearchServer.DeleteIndexAsync(index);
            return result;
        }
        #endregion
    }

    public class Persons
    {
        public string user { get; set; }
        public string message { get; set; }

        public DateTime post_date { get; set; }
        public List<string> albums { get; set; }
    }
}
