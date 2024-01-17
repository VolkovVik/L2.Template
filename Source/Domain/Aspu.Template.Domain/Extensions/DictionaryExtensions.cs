namespace Vekas.Aspu.L2.Domain.Common;

public static class DictionaryExtensions
{
    public static TValue? GetSafeValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
    {
        var value = dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        return value;
    }
}
