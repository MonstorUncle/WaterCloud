using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chloe;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using WaterCloud.Code;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.AutoJob
{
    public class JobCenter : IHostedService
    {
        private OpenJobsService _service;
        private IScheduler _scheduler;

        public JobCenter(OpenJobsService service, ISchedulerFactory schedulerFactory, IJobFactory iocJobfactory)
        {
            _service = service;
            _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            _scheduler.JobFactory = iocJobfactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            List<OpenJobEntity> obj = await _service.GetList(null);
            obj = obj.Where(a => a.F_EnabledMark == true).ToList();
            if (obj.Count > 0)
            {
                await AddScheduleJob(obj);
            }
            await _scheduler.Start();
            //if (!GlobalContext.SystemConfig.Debug)
            //{
            //    List<OpenJobEntity> obj = await new OpenJobService().GetList(null);
            //    if (obj.Count>0)
            //    {
            //        AddScheduleJob(obj);
            //    }
            //}
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ClearScheduleJob();
        }
        #region 添加任务计划
        /// <summary>
        /// 添加任务计划
        /// </summary>
        /// <returns></returns>
        private async Task AddScheduleJob(List<OpenJobEntity> entityList)
        {
            try
            {
                foreach (OpenJobEntity entity in entityList)
                {
                    entity.F_StarRunTime = DateTime.Now;
                    entity.F_EndRunTime = DateTime.Now.AddSeconds(-1);
                    DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(entity.F_StarRunTime, 1);
                    DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(DateTime.MaxValue.AddDays(-1), 1);
                    await _service.SubmitForm(entity, entity.F_Id);
                    IJobDetail job = JobBuilder.Create<JobExecute>().WithIdentity(entity.F_JobName, entity.F_JobGroup).Build();
                    job.JobDataMap.Add("F_Id", entity.F_Id);

                    ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                 .StartAt(starRunTime)
                                                 .EndAt(endRunTime)
                                                 .WithIdentity(entity.F_JobName, entity.F_JobGroup)
                                                 .WithCronSchedule(entity.F_CronExpress)
                                                 .Build();
                    await _scheduler.ScheduleJob(job, trigger);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteWithTime(ex);
            }
        }
        #endregion

        #region 清除任务计划
        /// <summary>
        /// 清除任务计划
        /// </summary>
        /// <returns></returns>
        public async Task ClearScheduleJob()
        {
            try
            {
                await _scheduler.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.WriteWithTime(ex);
            }
        }
        #endregion
    }
}
