using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shop.Infrastructure.Helpers;

public static class Md5Helper
{
    /// <summary>
    /// MD5 Encrypt
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Encrypt(string str, Encoding encoding = null)
    {
        if (encoding == null)
            encoding = Encoding.UTF8;
        MD5 md5 = new MD5CryptoServiceProvider();
        var hashBytes = md5.ComputeHash(encoding.GetBytes(str));
        var sb = new StringBuilder(32);
        for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("x").PadLeft(2, '0'));
        return sb.ToString();
    }

    /// <summary>
    /// MD5 Encrypt
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string Encrypt(byte[] bytes)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        var hashBytes = md5.ComputeHash(bytes);
        var sb = new StringBuilder(32);
        for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("x").PadLeft(2, '0'));
        return sb.ToString();
    }

    /// <summary>
    /// MD5 Encrypt
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string Encrypt(Stream stream)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        var hashBytes = md5.ComputeHash(stream);
        var sb = new StringBuilder(32);
        for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("x").PadLeft(2, '0'));
        return sb.ToString();
    }
}