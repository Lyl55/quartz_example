using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace quartz_example
{
    public class ExecuteJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.MergedJobDataMap;
                var plugin = (ICustomJob)dataMap.Get("Plugin");
                var job = (UserJob)dataMap.Get("UserJob");
                plugin.Execute(job);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                var trigger = context.Trigger;
                //xeta olsa trigeri durdur
                context.Scheduler.PauseTrigger(trigger.Key);
                Console.WriteLine($"{trigger.Key} is stopped");
                return Task.FromResult(TaskStatus.Faulted);
            }
        }
    }
    public class GlobalJobListener : IJobListener
    {
        public string Name => "SchedulerListener";

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job {1} time   ({2})  rejected", Name, DateTime.Now, context.JobDetail.Key);
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job {1} time  ({2}) will be worked", Name, DateTime.Now, context.JobDetail.Key);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("{0} Job {1} time  ({2})  was worked", Name, DateTime.Now, context.JobDetail.Key);
        }
    }
}
