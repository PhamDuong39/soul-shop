using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Abstractions.ViewModels;

/// <summary>
///
/// </summary>
public class RegisterByEmailParam
{
    [Required(ErrorMessage = "Please enter password")]
    [StringLength(100, ErrorMessage = "Password length 6-32 characters", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The passwords entered twice do not matc")]
    public string ConfirmPassword { get; set; }

    [EmailAddress] public string Email { get; set; }

    [Required(ErrorMessage = "Please enter phone number")]
    //validate phone number vietnam
    [RegularExpression(@"^((\+84|0)[2|3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }
}

