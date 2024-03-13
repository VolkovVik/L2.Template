using Aspu.Template.Application.Quartz.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Aspu.Template.Application.Quartz;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationQuartzServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var key = new JobKey("First test");
            q.AddJob<FirstTestJob>(options => options.WithIdentity(key));
            q.AddTrigger(options => options.ForJob(key)
                .WithIdentity(key.Name + "trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(5))
                .RepeatForever()));
        });
        services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        return services;
    }
}
