using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class RegisterByPhoneParam
{
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number format error")]
    public string Phone { get; set; }


    [Required(ErrorMessage = "captcha is not format")]
    public string Captcha { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "Password was be in 6-32 character", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and ConfirmPassword do not match.")]
    public string ConfirmPassword { get; set; }

    //[EmailAddress]
    public string Email { get; set; }
}