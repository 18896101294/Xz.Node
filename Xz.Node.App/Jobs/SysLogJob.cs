using System.Threading.Tasks;
using Quartz;
using Xz.Node.App.SysLogs;
using Xz.Node.Framework.Common;

namespace Xz.Node.App.Jobs
{
    /// <summary>
    /// 系统任务日志job
    /// </summary>
    public class SysLogJob : IJob
    {
        private SysLogApp _sysLogApp;
        private OpenJobApp _openJobApp;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysLogApp"></param>
        /// <param name="openJobApp"></param>
        public SysLogJob(SysLogApp sysLogApp, OpenJobApp openJobApp)
        {
            _sysLogApp = sysLogApp;
            _openJobApp = openJobApp;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jobId = context.MergedJobDataMap.GetString(Define.JOBMAPKEY);
            //todo:这里可以加入自己的自动任务逻辑
            _openJobApp.RecordRun(jobId);
            return Task.Delay(1);
        }
    }
}