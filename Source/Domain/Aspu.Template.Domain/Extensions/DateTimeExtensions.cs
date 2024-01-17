namespace Aspu.Template.Domain.Extensions;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

    public static DateTime GetLocalDate(this DateTime date, TimeZoneInfo? timeZone = null)
    {
        timeZone ??= LocalTimeZone;
        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var value = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcDate, timeZone.Id);
        return value;
    }
}
