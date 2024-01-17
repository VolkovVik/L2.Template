using System.Globalization;

namespace Aspu.Template.Domain.Extensions;

public static class BaseExtensions
{
    public static string? GetValue(string? value, string defaultValue) =>
        string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    public static DateTime? GetUtcDate(string? value, DateTime? defaultValue) =>
        string.IsNullOrWhiteSpace(value) || !DateTime.TryParse(value, CultureInfo.CurrentCulture, out var result) ? defaultValue : result.ToUniversalTime();

    public static string? GetLocalDate(DateTime? value, string format) =>
        !value.HasValue ? null : value.Value.GetLocalDate().ToString(format);

    public static Guid? GetGuidValue(string? value, Guid? defaultValue) =>
        string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var result) ? defaultValue : result;

    public static string? GetGuidValue(Guid? value, string? defaultValue) =>
        value.HasValue ? value.Value.ToString() : defaultValue;

    public static string? GetEnumValue<TEnum>(TEnum? value, string defaultValue) where TEnum : struct =>
        value == null ? defaultValue : Enum.GetName(typeof(TEnum), value);

    public static TEnum? GetEnumValue<TEnum>(string? value, TEnum? defaultValue) where TEnum : struct =>
        string.IsNullOrWhiteSpace(value) ||
        !Enum.IsDefined(typeof(TEnum), value) ||
        !Enum.TryParse<TEnum>(value, true, out var result) ? defaultValue : result;
}
