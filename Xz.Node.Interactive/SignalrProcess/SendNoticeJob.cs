using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.User;
using Xz.Node.App.System.Notice;
using Xz.Node.App.System.Notice.Enums;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Interactive.Hubs;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.Interactive.SignalrProcess
{
    public class SendNoticeJob : IHostedService, IDisposable
    {
        private readonly ILogger<SendNoticeJob> _log;
        private readonly IHubContext<ServiceHub> _hubContext;
        private readonly SystemNoticeApp _systemNoticeApp;
        private readonly UserApp _userApp;
        private readonly RevelanceApp _revelanceApp;
        /// <summary>
        /// 系统通知推送job
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="log"></param>
        /// <param name="systemNoticeApp"></param>
        /// <param name="userApp"></param>
        /// <param name="revelanceApp"></param>
        public SendNoticeJob(IHubContext<ServiceHub> hubContext,
            ILogger<SendNoticeJob> log,
            SystemNoticeApp systemNoticeApp,
            UserApp userApp,
            RevelanceApp revelanceApp)
        {
            _hubContext = hubContext;
            _log = log;
            _systemNoticeApp = systemNoticeApp;
            _userApp = userApp;
            _revelanceApp = revelanceApp;
        }
        /// <summary>
        /// 推送系统通知
        /// </summary>
        void SendNotice()
        {
            while (true)
            {
                try
                {
                    //获取需要执行的系统通知配置
                    var execDatas = _systemNoticeApp.GetExecDats();
                    if (execDatas.Count() > 0)
                    {
                        //获取所有用户
                        var allUser = _userApp.LoadUserAll();

                        var updateDatas = new List<System_NoticeInfo>();
                        foreach (var item in execDatas)
                        {
                            var typeName = "系统通知";
                            switch (item.Type)
                            {
                                case (int)NoticeTypeEnum.BySystem:
                                    typeName = NoticeTypeEnum.BySystem.GetDescription();
                                    break;
                                case (int)NoticeTypeEnum.ByUpdate:
                                    typeName = NoticeTypeEnum.ByUpdate.GetDescription();
                                    break;
                                default:
                                    break;
                            }
                            var data = new
                            {
                                Title = item.Titile,
                                Content = $"{typeName}：{item.Content}",
                                IsHtml = item.IsHtml,
                            };
                            switch (item.RangeType)
                            {
                                //通知所有人
                                case (int)NoticeRangeTypeEnum.All:
                                    _hubContext.Clients.All.SendAsync("SendNoticeAll", data);
                                    item.IsExec = true;
                                    updateDatas.Add(item);
                                    break;
                                //按部门通知
                                case (int)NoticeRangeTypeEnum.ByOrgs:
                                    var orgIds = item.RangeIds.Split(',');
                                    if (orgIds.Length > 0)
                                    {
                                        var orgUserIds = _revelanceApp.Get(Define.USERORG, false, orgIds);
                                        var sendUsers = allUser.Where(o => orgUserIds.Contains(o.Id));
                                        foreach (var sendUser in sendUsers)
                                        {
                                            _hubContext.Clients.All.SendAsync($"SendUserNotice_{sendUser.Account}", data);
                                        }
                                    }
                                    item.IsExec = true;
                                    updateDatas.Add(item);
                                    break;
                                //按角色通知
                                case (int)NoticeRangeTypeEnum.ByRoles:
                                    var roleIds = item.RangeIds.Split(',');
                                    if (roleIds.Length > 0)
                                    {
                                        var roleUserIds = _revelanceApp.Get(Define.USERROLE, false, roleIds);
                                        var sendUsers = allUser.Where(o => roleUserIds.Contains(o.Id));
                                        foreach (var sendUser in sendUsers)
                                        {
                                            _hubContext.Clients.All.SendAsync($"SendUserNotice_{sendUser.Account}", data);
                                        }
                                    }
                                    item.IsExec = true;
                                    updateDatas.Add(item);
                                    break;
                                //按用户通知
                                case (int)NoticeRangeTypeEnum.ByUsers:
                                    var userIds = item.RangeIds.Split(',');
                                    if (userIds.Length > 0)
                                    {
                                        var sendUsers = allUser.Where(o => userIds.Contains(o.Id));
                                        foreach (var sendUser in sendUsers)
                                        {
                                            _hubContext.Clients.All.SendAsync($"SendUserNotice_{sendUser.Account}", data);
                                        }
                                    }
                                    item.IsExec = true;
                                    updateDatas.Add(item);
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (updateDatas.Count() > 0)
                        {
                            _systemNoticeApp.Update(updateDatas);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.Message);
                }
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Task taskSendNotice = new Task(SendNotice);

                taskSendNotice.Start();

                _log.LogInformation("SendNoticeJob服务启动成功");
            }
            catch (Exception ex)
            {
                _log.LogError($"SendNoticeJob服务器启动失败，错误消息：{ex.Message}");
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

        public void Dispose()
        {
        }
    }
}

