using System.Security.Cryptography;
using System.Text;

namespace Soul.Shop.Module.Payment.Abstractions.Helper
{
    public static class SecurityHelper
    {
        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToLower();
            }
        }
    }
}
