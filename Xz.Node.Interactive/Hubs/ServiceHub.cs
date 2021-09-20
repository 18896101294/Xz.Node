using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xz.Node.Interactive.Hubs
{
    /// <summary>
    /// SignalR接线器
    /// </summary>
    public class ServiceHub : Hub
    {
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
            Trace.WriteLine("客户端连接成功");
            return base.OnConnectedAsync();
        }
    }
}
