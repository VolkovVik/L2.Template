using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;

namespace Aspu.Template.Application.Quartz;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationQuartzServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        ///var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var jobs = assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => typeof(IJob).IsAssignableFrom(x))
            .ToList();

        var jobNames = jobs.Select(x => x.Name).ToList();

        var jobConfigs = configuration.GetSection("Quartz")?
            .Get<List<QuartzSettings>>()?
            .Where(x => x.Enabled)
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Where(x => jobNames.Contains(x.Name)).ToArray() ?? [];

        services.AddQuartz(q =>
        {
            foreach (var config in jobConfigs)
            {
                var jobType = jobs.Find(x => x.Name == config?.Name);
                if (jobType == null) continue;

                var jobKey = new JobKey($"{config.Name}JobKey", $"{config.Name}JobGroup");
                var triggerKey = new TriggerKey($"{config.Name}TriggerKey", $"{config.Name}TriggerGroup");

                q.AddJob(jobType, jobKey, options => options
                    .WithIdentity(jobKey)
                    .UsingJobData("Counter1", 123)
                );
                q.AddTrigger(options => options
                    .ForJob(jobKey)
                    .WithIdentity(triggerKey)
                    .UsingJobData("Counter2", 321)
                    .GetTriggerConfiguratorBuilder(config)
                );
            }
        });
        services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        return services;
    }
}
