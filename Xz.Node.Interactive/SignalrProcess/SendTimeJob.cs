using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xz.Node.Interactive.Hubs;

namespace Xz.Node.Interactive.SignalrProcess
{
    public class SendTimeJob : IHostedService
    {
        private readonly ILogger<SendTimeJob> _log;
        private readonly IHubContext<ServiceHub> _hubContext;

        public SendTimeJob(IHubContext<ServiceHub> hubContext,
            ILogger<SendTimeJob> log)
        {
            _hubContext = hubContext;
            _log = log;
        }
        /// <summary>
        /// 推送时间
        /// </summary>
        void SendTime()
        {
            while (true)
            {
                try
                {
                    _hubContext.Clients.All.SendAsync("GetHubTime", DateTime.Now.ToString("yyyy-MM-dd：HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.Message);
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Task taskSendBdcTime = new Task(SendTime);

                taskSendBdcTime.Start();

                _log.LogInformation("SendTimeJob服务启动成功");
            }
            catch (Exception ex)
            {
                _log.LogError($"SendTimeJob服务器启动失败，错误消息：{ex.Message}");
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
