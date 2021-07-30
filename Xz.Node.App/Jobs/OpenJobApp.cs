using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xz.Node.App.Base;
using Xz.Node.App.Extensions;
using Xz.Node.App.Interface;
using Xz.Node.App.Jobs.Request;
using Xz.Node.App.SysLogs;
using Xz.Node.Framework.Const;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Jobs
{
    /// <summary>
    /// 系统定时任务管理
    /// </summary>
    public class OpenJobApp : BaseStringApp<System_OpenJobInfo, XzDbContext>
    {
        private SysLogApp _sysLogApp;
        private IScheduler _scheduler;
        private ILogger<OpenJobApp> _logger;
        /// <summary>
        /// 系统定时任务管理构造函数
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        /// <param name="sysLogApp"></param>
        /// <param name="scheduler"></param>
        /// <param name="logger"></param>
        public OpenJobApp(IUnitWork<XzDbContext> unitWork, IRepository<System_OpenJobInfo, XzDbContext> repository,
            IAuth auth, SysLogApp sysLogApp, IScheduler scheduler, ILogger<OpenJobApp> logger) : base(unitWork,
            repository, auth)
        {
            _sysLogApp = sysLogApp;
            _scheduler = scheduler;
            _logger = logger;
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        public async Task<TableData> Load(QueryOpenJobListReq request)
        {
            var result = new TableData();
            var objs = Repository.Find(null);
            if (!string.IsNullOrEmpty(request.key))
            {
                objs = objs.Where(u => u.Id.Contains(request.key));
            }

            result.data = objs.OrderBy(u => u.Id)
                .Skip((request.page - 1) * request.limit)
                .Take(request.limit);
            result.count = objs.Count();
            await Task.CompletedTask;
            return result;
        }

        /// <summary>
        /// 启动所有状态为正在运行的任务
        /// <para>通常应用在系统加载的时候</para>
        /// </summary>
        /// <returns></returns>
        public async Task StartAll()
        {
            var jobs = Repository.Find(u => u.Status == (int)JobStatus.Running);
            foreach (var job in jobs)
            {
                job.Start(_scheduler);
            }
            _logger.LogInformation("所有状态为正在运行的任务已启动");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="req"></param>
        public void Add(AddOrUpdateOpenJobReq req)
        {
            var obj = req.MapTo<System_OpenJobInfo>();
            obj.CreateTime = DateTime.Now;
            var user = _auth.GetCurrentUser().User;
            obj.CreateUserId = user.Id;
            obj.CreateUserName = user.Name;
            Repository.Add(obj);
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="obj"></param>
        public void Update(AddOrUpdateOpenJobReq obj)
        {
            var user = _auth.GetCurrentUser().User;
            UnitWork.Update<System_OpenJobInfo>(u => u.Id == obj.Id, u => new System_OpenJobInfo
            {
                JobName = obj.JobName,
                JobType = obj.JobType,
                JobCall = obj.JobCall,
                JobCallParams = obj.JobCallParams,
                Cron = obj.Cron,
                Status = obj.Status,
                Remark = obj.Remark,
                UpdateTime = DateTime.Now,
                UpdateUserId = user.Id,
                UpdateUserName = user.Name
            });
        }

        #region 定时任务运行相关操作
        /// <summary>
        /// 返回系统的job接口
        /// </summary>
        /// <returns></returns>
        public List<string> QueryLocalHandlers()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces()
                    .Contains(typeof(IJob))))
                .ToArray();
            return types.Select(u => u.FullName).ToList();
        }

        /// <summary>
        /// 修改服务状态
        /// </summary>
        /// <param name="req"></param>
        public void ChangeJobStatus(ChangeJobStatusReq req)
        {
            var job = Repository.FirstOrDefault(u => u.Id == req.Id);
            if (job == null)
            {
                throw new Exception("任务不存在");
            }


            if (req.Status == (int)JobStatus.NotRun) //停止
            {
                job.Stop(_scheduler);
            }
            else //启动
            {
                job.Start(_scheduler);
            }


            var user = _auth.GetCurrentUser().User;

            job.Status = req.Status;
            job.UpdateTime = DateTime.Now;
            job.UpdateUserId = user.Id;
            job.UpdateUserName = user.Name;
            Repository.Update(job);
        }

        /// <summary>
        /// 记录任务运行结果
        /// </summary>
        /// <param name="jobId"></param>
        public void RecordRun(string jobId)
        {
            var job = Repository.FirstOrDefault(u => u.Id == jobId);
            if (job == null)
            {
                _sysLogApp.Add(new System_SysLogInfo
                {
                    TypeName = "定时任务",
                    TypeId = "AUTOJOB",
                    Content = $"未能找到定时任务：{jobId}"
                });
                return;
            }

            job.RunCount++;
            job.LastRunTime = DateTime.Now;
            Repository.Update(job);

            _sysLogApp.Add(new System_SysLogInfo
            {
                CreateName = "Quartz",
                CreateId = "Quartz",
                TypeName = "定时任务",
                TypeId = "AUTOJOB",
                Content = $"运行了自动任务：{job.JobName}"
            });
            _logger.LogInformation($"运行了自动任务：{job.JobName}");
        }

        #endregion
    }
}
