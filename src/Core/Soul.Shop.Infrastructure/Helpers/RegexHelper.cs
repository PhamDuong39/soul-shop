using System.Text.RegularExpressions;

namespace Soul.Shop.Infrastructure.Helpers;

public static class RegexHelper
{
    //email pattern
    private const string PatternEmail = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";

    //phone vietnam pattern 10 number phone +84 or 11 number phone 0
    private const string PatternPhone = @"^\d{10}$|^\d{11}$";


    public static (bool Succeeded, string Message) VerifyEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return (false, "Email cannot be empty");
        var regex = new Regex(PatternEmail);
        if (!regex.IsMatch(input))
            return (false, "Email format is incorrect");
        return (true, "Email format is correct");
    }

    public static (bool Succeeded, string Message) VerifyPhone(string input)
    {
        if (string.IsNullOrWhiteSpace(input.Trim()))
            return (false, "Please enter your phone number");
        var regex = new Regex(PatternPhone);
        if (!regex.IsMatch(input.Trim()))
            return (false, "Phone number format is incorrect");
        return (true, "Phone number format is correct");
    }
}