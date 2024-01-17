using System.ComponentModel.DataAnnotations;

namespace Aspu.Template.Domain.Extensions;

public static class EnumsExtensions
{
    public static TEnum[] EnumParse<TEnum>(this IEnumerable<string> items) where TEnum : struct, Enum
    {
        var list = items
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Where(x => Enum.IsDefined(typeof(TEnum), x))
            .Select(x => Enum.Parse(typeof(TEnum), x, true))
            .OfType<TEnum>()
            .ToArray();
        return list;
    }

    public static TEnum? EnumParse<TEnum>(this string item, TEnum? defaultValue = default) where TEnum : struct, Enum
    {
        var value = string.IsNullOrWhiteSpace(item) || !Enum.IsDefined(typeof(TEnum), item)
            ? defaultValue
            : (TEnum)Enum.Parse(typeof(TEnum), item, true);
        return value;
    }

    public static string ToFriendlyString(this Enum code)
    {
        var value = Enum.GetName(code.GetType(), code);
        return value ?? string.Empty;
    }

    public static T[] GetAllValues<T>()
    {
        var items = Enum.GetValues(typeof(T)).OfType<T>().ToArray();
        return items;
    }

    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());

        if (memberInfo.Length <= 0) return enumValue.ToString();
        var attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attrs.Length <= 0) return enumValue.ToString();

        var displayAttribute = (DisplayAttribute)attrs[0];
        return displayAttribute?.Name ?? string.Empty;
    }
}
