using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class LoginPhoneParam
{
    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number format error")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Code  is not format")]
    public string Code { get; set; }

    public bool RememberMe { get; set; }
}