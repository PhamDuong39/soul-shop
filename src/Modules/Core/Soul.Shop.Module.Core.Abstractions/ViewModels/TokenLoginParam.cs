using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class TokenLoginParam
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}