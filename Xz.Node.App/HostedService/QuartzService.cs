using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xz.Node.App.Jobs;

namespace Xz.Node.App.HostedService
{
    /// <summary>
    /// 自启动服务，本服务用于启动所有状态为【正在运行】的定时任务
    /// </summary>
    public class QuartzService : IHostedService, IDisposable
    {
        private readonly ILogger<QuartzService> _logger;
        private IScheduler _scheduler;
        private OpenJobApp _openJobApp;
        /// <summary>
        /// 自启动服务
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="scheduler"></param>
        /// <param name="openJobApp"></param>
        public QuartzService(ILogger<QuartzService> logger, IScheduler scheduler, OpenJobApp openJobApp)
        {
            _logger = logger;
            _scheduler = scheduler;
            _openJobApp = openJobApp;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Start();
            await _openJobApp.StartAll();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown();
            _logger.LogInformation("关闭定时job");
        }

        public void Dispose()
        {

        }
    }
}
