using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class AddPhoneParam
{
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number format error")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Code number is required")]
    public string Code { get; set; }
}