using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using quartz_example.Jobs;

namespace quartz_example
{
    class Program
    {
        static IScheduler scheduler;
        static async Task Main(string[] args)
        {
            var context = new StdSchedulerFactory();
            scheduler = await context.GetScheduler();
            if (!scheduler.IsStarted)
            {
                await scheduler.Start();
                Console.WriteLine("Scheduler started to work");
            }

            var paths = Directory.GetFiles(@"C:\Users\user\source\repos\quartz_example\quartz_example\Jobs\");
            var jobs = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterface(nameof(ICustomJob)) != null)
                .ToList();
            var joblist = await CreateJobs(jobs);
            await CreateTrigger(joblist);
            Console.ReadLine();
        }
        
        /// <summary>
        /// Job created
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        private static async Task<List<CustomJob>> CreateJobs(List<Type> jobs)
        {
            var customJob = new List<CustomJob>();
            jobs.ForEach(async job =>
            {
                var instance = (ICustomJob)Activator.CreateInstance(job);
                customJob.Add(new CustomJob
                {
                    Id = instance.Id,
                    Instance = instance,
                    JobType = job,
                    Name = job.Name
                });
                //unique
                JobKey jobKey = new JobKey(instance.Name);
                var jobIsExist = await scheduler.CheckExists(jobKey);
                if (!jobIsExist) 
                {
                    IJobDetail jobDetail=JobBuilder.Create<ExecuteJob>().WithIdentity(jobKey).Build();
                    await scheduler.AddJob(jobDetail, true, true, CancellationToken.None);

                    //Quartz Middleware
                    GlobalJobListener globalJobListener = new GlobalJobListener();
                    scheduler.ListenerManager.AddJobListener(new GlobalJobListener(), GroupMatcher<JobKey>.AnyGroup());
                }
            });
            return customJob;
        }

        /// <summary>
        /// Trigger created
        /// </summary>
        /// <param name="customJobs"></param>
        /// <returns></returns>
        private static async Task CreateTrigger(List<CustomJob> customJobs)
        {
            var userList = new UserJob();

            userList.UserJobList.ForEach(async userJob =>
            {
                var jobKey = new JobKey(userJob.JobName);
                var triggerKey = new TriggerKey(userJob.Id.ToString());
                var triggerIsExist = await scheduler.CheckExists(jobKey);
                if (triggerIsExist)
                {
                    var currentJob = customJobs.FirstOrDefault(x => x.Id == userJob.JobId);
                    JobDataMap jobDataMap = new JobDataMap();
                    jobDataMap.Add("Plugin", currentJob.Instance);
                    jobDataMap.Add("UserJob", userJob);
                    ITrigger jobTrigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .UsingJobData(jobDataMap)
                        .ForJob(jobKey)
                        .WithCronSchedule(userJob.Cron)
                        .Build();
                    var result = await scheduler.ScheduleJob(jobTrigger);
                }
            });
        }
    }
}
