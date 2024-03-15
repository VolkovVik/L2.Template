using System.Text.Json.Serialization;

namespace Aspu.Template.Application.Quartz;

public class QuartzSettings
{
    public required string Name { get; set; }
    public bool Enabled { get; set; }
    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }

    public string? CronValue { get; set; }

    [JsonIgnore]
    public TimeSpan? Period => GetPeriod();
    public int? DaysPeriod { get; set; }
    public int? HoursPeriod { get; set; }
    public int? MinutesPeriod { get; set; }
    public int? SecondsPeriod { get; set; }
    public int? MillisecondsPeriod { get; set; }
    public int RepeatCount { get; set; } = -1;

    public TimeSpan? GetPeriod()
    {
        var value = SumPeriod(null, DaysPeriod, TimeSpan.FromDays);
        value = SumPeriod(value, HoursPeriod, TimeSpan.FromHours);
        value = SumPeriod(value, MinutesPeriod, TimeSpan.FromMinutes);
        value = SumPeriod(value, SecondsPeriod, TimeSpan.FromSeconds);
        value = SumPeriod(value, MillisecondsPeriod, TimeSpan.FromMilliseconds);
        return value;
    }

    public static TimeSpan? SumPeriod(TimeSpan? value, int? period, Func<double, TimeSpan> func)
    {
        var add = !period.HasValue || period.Value < 1 ? (TimeSpan?)null : func(period.Value);
        if (!add.HasValue) return value;

        value = value == null ? add : value + add;
        return value;
    }
}
