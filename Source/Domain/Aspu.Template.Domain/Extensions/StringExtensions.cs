namespace Vekas.Aspu.L2.Domain.Common;

public static class StringExtensions
{
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string ReplaceFirstOccurrence(this string Source, string Find, string Replace)
    {
        int Place = Source.IndexOf(Find);
        string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
        return result;
    }

    public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
    {
        int Place = Source.LastIndexOf(Find);
        string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
        return result;
    }

    public static int GetInt(this string value, int minLimit, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var valueInt)) return defaultValue;

        if (valueInt < minLimit) valueInt = minLimit;
        return valueInt;
    }
}
