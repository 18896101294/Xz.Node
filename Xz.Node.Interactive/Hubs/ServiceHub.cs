using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xz.Node.Interactive.Hubs
{
    /// <summary>
    /// SignalR接线器
    /// </summary>
    public class ServiceHub : Hub
    {
        private readonly ILogger<ServiceHub> _log;
        public ServiceHub(ILogger<ServiceHub> log)
        {
            _log = log;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 客户端连接的时候调用
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            _log.LogInformation("客户端连接成功");
            Trace.WriteLine("客户端连接成功");
            return base.OnConnectedAsync();
        }
    }
}
