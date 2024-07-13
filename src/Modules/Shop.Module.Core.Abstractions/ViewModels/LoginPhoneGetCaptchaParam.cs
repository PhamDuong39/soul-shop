using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.Abstractions.ViewModels;

public class LoginEmailGetCaptchaParam
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}
