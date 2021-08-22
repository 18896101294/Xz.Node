using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using System.Threading.Tasks;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// 全局异常捕捉，自定义异常返回格式
    /// </summary>
    public class GloabExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<GloabExceptionFilterAttribute> _log;
        private readonly IWebHostEnvironment _environment;
        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="environment"></param>
        public GloabExceptionFilterAttribute(ILogger<GloabExceptionFilterAttribute> log,
            IWebHostEnvironment environment)
        {
            _log = log;
            _environment = environment;
        }

        /// <summary>
        /// 同步异常捕捉
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            if (_environment.IsDevelopment())
            {
                //把堆栈信息写到MiniProfiler
                var profiler = MiniProfiler.Current;
                /*
                 * 第一个参数是节点名称
                 * 第二个参数是显示的信息
                 * 第三个参数是显示这个信息所需的最短执行时间：比如设置为1则代表程序最少要允许1秒这个信息才会显示到MiniProfiler上
                 */
                using (var step = profiler.CustomTimingIf("error", $"StackTrace : {context.Exception.StackTrace} \n Message:{context.Exception.Message}", 0))
                {
                    step.Errored = true;
                }
            }

            var responseCode = 500;
            if (context.Exception is InfoException)
            {
                responseCode = ((InfoException)context.Exception).ErrorCode;
            }

            var data = new ResultInfo<object>
            {
                Code = responseCode,
                Message = context.Exception.Message
            };

            context.Result = new OkObjectResult(data);
            context.ExceptionHandled = true;
            if (!(context.Exception is InfoException))
            {
                _log.LogError(context.Exception, context.Exception.Message);
            }
        }

        /// <summary>
        /// 异步异常捕捉，如果不重写此异步方法，则会调用同步的方法。会默认异步的优先调用。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            base.OnException(context);

            if (_environment.IsDevelopment())
            {
                //把异常的堆栈信息写到MiniProfiler
                var profiler = MiniProfiler.Current;
                /*
                 * 第一个参数是节点名称
                 * 第二个参数是显示的信息
                 * 第三个参数是显示这个信息所需的最短执行时间：比如设置为1则代表程序最少要允许1秒这个信息才会显示到MiniProfiler上
                 */
                using (var step = profiler.CustomTimingIf("error", $"StackTrace : {context.Exception.StackTrace} \n Message:{context.Exception.Message}", 0))
                {
                    step.Errored = true;
                }
            }

            var responseCode = 500;
            if (context.Exception is InfoException)
            {
                responseCode = ((InfoException)context.Exception).ErrorCode;
            }

            var data = new
            {
                code = responseCode,
                success = false,
                message = context.Exception.Message
            };

            context.Result = new OkObjectResult(data);
            context.ExceptionHandled = true;
            if (!(context.Exception is InfoException))
            {
                _log.LogError(context.Exception, context.Exception.Message);
            }
            return Task.CompletedTask;
        }
    }
}
