using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class AddEmailPutParam
{
    [Required] public int UserId { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Email address format error")]
    public string Email { get; set; }

    [Required()] public string Code { get; set; }
}