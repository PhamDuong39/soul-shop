namespace Soul.Shop.Infrastructure.Extensions;

public static class DictionaryExtensions
{
    internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T? value)
    {
        if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T variable)
        {
            value = variable;
            return true;
        }

        value = default;
        return false;
    }

    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    private static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TKey, TValue> factory)
    {
        if (dictionary.TryGetValue(key, out var obj)) return obj;

        return dictionary[key] = factory(key);
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, _ => factory());
    }
}