using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class LoginPhoneGetCaptchaParam
{
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number format error")]
    public string Phone { get; set; }
}