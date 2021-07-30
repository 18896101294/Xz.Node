using Quartz;
using System;
using System.Linq;
using Xz.Node.Framework.Common;
using Xz.Node.Repository.Domain.System;

namespace Xz.Node.App.Extensions
{
    /// <summary>
    /// 定时任务扩展
    /// </summary>
    public static class OpenJobExtensions
    {
        /// <summary>
        /// 启动定时任务
        /// </summary>
        /// <param name="job"></param>
        /// <param name="scheduler">一个Quartz Scheduler</param>
        public static void Start(this System_OpenJobInfo job, IScheduler scheduler)
        {
            var jobBuilderType = typeof(JobBuilder);
            var method = jobBuilderType.GetMethods().FirstOrDefault(
                    x => x.Name.Equals("Create", StringComparison.OrdinalIgnoreCase) &&
                         x.IsGenericMethod && x.GetParameters().Length == 0)
                ?.MakeGenericMethod(Type.GetType(job.JobCall));

            var jobBuilder = (JobBuilder)method.Invoke(null, null);

            IJobDetail jobDetail = jobBuilder.WithIdentity(job.Id).Build();
            jobDetail.JobDataMap[Define.JOBMAPKEY] = job.Id; //传递job信息
            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(job.Cron)
                .WithIdentity(job.Id)
                .StartNow()
                .Build();
            scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 停止一个定时任务
        /// </summary>
        /// <param name="job"></param>
        /// <param name="scheduler"></param>
        public static void Stop(this System_OpenJobInfo job, IScheduler scheduler)
        {
            TriggerKey triggerKey = new TriggerKey(job.Id);
            // 停止触发器
            scheduler.PauseTrigger(triggerKey);
            // 移除触发器
            scheduler.UnscheduleJob(triggerKey);
            // 删除任务
            scheduler.DeleteJob(new JobKey(job.Id));
        }
    }
}
