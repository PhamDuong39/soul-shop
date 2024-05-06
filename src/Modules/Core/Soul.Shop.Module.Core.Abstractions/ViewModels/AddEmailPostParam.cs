using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class AddEmailPostParam
{
    [Required]
    [EmailAddress(ErrorMessage = "Email address format error")]
    public string Email { get; set; }
}