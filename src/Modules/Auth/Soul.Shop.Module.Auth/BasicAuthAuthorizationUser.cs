using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Soul.Shop.Module.Auth;

public class BasicAuthAuthorizationUser
{
    public string Login { get; set; }

    public byte[] Password { get; set; }

    public string PasswordClear
    {
        set
        {
            using (var cryptoProvider = SHA1.Create())
            {
                Password = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }
    }

    public bool Validate(string login, string password, bool loginCaseSensitive)
    {
        if (string.IsNullOrWhiteSpace(login) == true)
            throw new ArgumentNullException("login");

        if (string.IsNullOrWhiteSpace(password) == true)
            throw new ArgumentNullException("password");

        if (login.Equals(Login,
                loginCaseSensitive ? StringComparison.CurrentCulture : StringComparison.OrdinalIgnoreCase) == true)
        {
            using var cryptoProvider = SHA1.Create();
            var passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
            return StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, Password);
        }
        else
            return false;
    }
}