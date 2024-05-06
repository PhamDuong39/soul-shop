using System.Globalization;
using System.Text;

namespace Soul.Shop.Infrastructure.Helpers;

public static class StringHelper
{
    public static string ToUrlFriendly(this string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Guid.NewGuid().ToString();

        name = name.ToLower();
        name = RemoveDiacritics(name);
        name = ConvertEdgeCases(name);
        name = name.Replace(" ", "-");
        name = name.Strip(c =>
            c != '-'
            && c != '_'
            && !char.IsLetter(c)
            && !char.IsDigit(c)
        );

        while (name.Contains("--"))
            name = name.Replace("--", "-");

        if (name.Length > 200)
            name = name[..200];

        return string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in from c in normalizedString
                 let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                 where unicodeCategory != UnicodeCategory.NonSpacingMark
                 select c)
            stringBuilder.Append(c);

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string Strip(this string subject, params char[]? stripped)
    {
        if (stripped == null || stripped.Length == 0 || string.IsNullOrEmpty(subject)) return subject;

        var result = new char[subject.Length];

        var cursor = 0;
        foreach (var current in subject.Where(current => Array.IndexOf(stripped, current) < 0))
            result[cursor++] = current;

        return new string(result, 0, cursor);
    }

    private static string Strip(this string subject, Func<char, bool> predicate)
    {
        var result = new char[subject.Length];

        var cursor = 0;
        foreach (var current in subject.Where(current => !predicate(current))) result[cursor++] = current;

        return new string(result, 0, cursor);
    }

    public static string TrimStart(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        var result = target;
        while (result.StartsWith(trimString)) result = result[trimString.Length..];

        return result;
    }

    public static string TrimEnd(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        var result = target;
        while (result.EndsWith(trimString)) result = result[..^trimString.Length];

        return result;
    }

    private static string ConvertEdgeCases(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text) sb.Append(ConvertEdgeCases(c));

        return sb.ToString();
    }

    private static string ConvertEdgeCases(char c)
    {
        var swap = c switch
        {
            'ı' => "i",
            'ł' or 'Ł' => "l",
            'đ' => "d",
            'ß' => "ss",
            'ø' => "o",
            'Þ' => "th",
            _ => c.ToString()
        };

        return swap;
    }


    public static string EmailEncryption(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;
        var split = email.Split('@');
        if (split.Length < 2) return email;
        var before = split[0];
        if (before.Length <= 0) return email;
        var len = before.Length / 3;
        len = len <= 0 ? 1 : len;

        email = before[..len] + "***" +
                before[^len..]
                + "@" + split[1];

        return email;
    }


    public static string PhoneEncryption(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;
        if (phone.Length <= 0) return phone;
        var len = phone.Length / 3;
        len = len <= 0 ? 1 : len;
        phone = phone[..len] + "***" + phone[^len..];

        return phone;
    }
}