using Quartz;
using Serilog;

namespace Aspu.Template.Application.Quartz.Jobs;

internal class SecondTestJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Log.Debug($"{nameof(SecondTestJob)}  {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}");
        return Task.CompletedTask;
    }
}
