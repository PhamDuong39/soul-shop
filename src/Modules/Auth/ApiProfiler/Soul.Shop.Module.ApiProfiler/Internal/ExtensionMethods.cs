using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Soul.Shop.Module.ApiProfiler.Internal;

public static partial class ExtensionMethods
{
    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool HasValue(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }


    public static string? Truncate(this string s, int maxLength)
    {
        return s?.Length > maxLength ? s[..maxLength] : s;
    }


    public static bool Contains(this string s, string value, StringComparison comparison)
    {
        return s.Contains(value, comparison);
    }

    public static string EnsureTrailingSlash(this string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return MyRegex().Replace(input, string.Empty) + "/";
    }


    public static string ToJson(this List<Guid> guids)
    {
        if (guids == null || guids.Count == 0) return "[]";

        var sb = new StringBuilder("[");
        for (var i = 0; i < guids.Count; i++)
        {
            sb.Append('"').Append(guids[i].ToString()).Append('"');
            if (i < guids.Count - 1) sb.Append(',');
        }

        sb.Append(']');
        return sb.ToString();
    }

    private static readonly JsonSerializerSettings defaultSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    private static readonly JsonSerializerSettings htmlEscapeSettings = new()
    {
        StringEscapeHandling = StringEscapeHandling.EscapeHtml,
        NullValueHandling = NullValueHandling.Ignore
    };

    public static string ToJson(this MiniProfiler profiler, bool htmlEscape = false)
    {
        return profiler != default
            ? htmlEscape
                ? JsonConvert.SerializeObject(profiler, htmlEscapeSettings)
                : JsonConvert.SerializeObject(profiler, defaultSettings)
            : null;
    }

    public static string ToJson(this object o)
    {
        return o != null ? JsonConvert.SerializeObject(o, defaultSettings) : null;
    }


    public static T FromJson<T>(this string s) where T : class
    {
        return !string.IsNullOrEmpty(s) ? JsonConvert.DeserializeObject<T>(s, defaultSettings) : null;
    }


    public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value)
    {
        value = default;
        return dict?.TryGetValue(key, out value) == true && dict.Remove(key);
    }

    [GeneratedRegex("/+$")]
    private static partial Regex MyRegex();
}