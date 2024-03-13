using Quartz;
using Serilog;

namespace Aspu.Template.Application.Quartz.Jobs;

[DisallowConcurrentExecution]
internal class FirstTestJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Debug($"{nameof(FirstTestJob)}  {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}");
        await Task.Delay(TimeSpan.FromSeconds(15));
        //return Task.CompletedTask;
    }
}
