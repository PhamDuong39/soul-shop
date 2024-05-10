using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class ResetPasswordPostParam
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }
}