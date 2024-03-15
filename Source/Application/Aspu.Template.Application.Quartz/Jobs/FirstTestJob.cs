using Quartz;
using Serilog;

namespace Aspu.Template.Application.Quartz.Jobs;

[PersistJobDataAfterExecution]
[DisallowConcurrentExecution]
public class FirstTestJob : IJob
{
    public const string JobDataCounter = "Counter1";
    public const string TriggerDataCounter = "Counter2";

    public int Counter1 { get; set; }
    public int Counter2 { get; set; }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await ExecuteInternal(context);

        }
        catch (JobExecutionException ex)
        {
            Log.Error(ex, $"{nameof(FirstTestJob)} error");
        }
    }

    public async Task ExecuteInternal(IJobExecutionContext context)
    {
        var key = context.JobDetail.Key;

        // note: use context.MergedJobDataMap in production code
        JobDataMap jobDataMap = context.JobDetail.JobDataMap;
        var counter1 = jobDataMap.GetIntValue(JobDataCounter);
        jobDataMap.PutAsString(JobDataCounter, counter1 + 1);

        JobDataMap triggerDataMap = context.Trigger.JobDataMap;
        var counter2 = triggerDataMap.GetIntValue(TriggerDataCounter);
        triggerDataMap.PutAsString(TriggerDataCounter, counter2 + 1);

        JobDataMap mergeDataMap = context.MergedJobDataMap;
        mergeDataMap.PutAsString(JobDataCounter, counter1 + 1);

        Log.Debug($"{nameof(FirstTestJob)}  {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}");
        Log.Debug($"Instance {key} => {Counter1}  {Counter2}");

        await Task.Delay(TimeSpan.FromSeconds(1));
        //return Task.CompletedTask;
    }
}
