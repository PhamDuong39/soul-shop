using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class AddPhoneGetCaptchaParam
{
    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Email address format error")]
    public string Phone { get; set; }
}