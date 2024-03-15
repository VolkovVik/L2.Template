using Quartz;

namespace Aspu.Template.Application.Quartz;

public static class QuartzExtension
{
    public static ITriggerConfigurator GetTriggerConfiguratorBuilder(this ITriggerConfigurator trigger, QuartzSettings config)
    {
        trigger = !config.StartAt.HasValue
            ? trigger.StartNow()
            : trigger.StartAt(config.StartAt.Value);

        trigger = !config.EndAt.HasValue
            ? trigger
            : trigger.EndAt(config.EndAt.Value);

        if (!string.IsNullOrWhiteSpace(config.CronValue))
            return trigger.WithCronSchedule(config.CronValue);

        if (!config.Period.HasValue)
            return trigger;

        trigger = config.RepeatCount < 0
            ? trigger.WithSimpleSchedule(x => x.WithInterval(config.Period.Value).RepeatForever())
            : trigger.WithSimpleSchedule(x => x.WithInterval(config.Period.Value).WithRepeatCount(config.RepeatCount));
        return trigger;
    }
}
