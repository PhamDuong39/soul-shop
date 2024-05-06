using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class RefreshTokenParam
{
    [Required] public string Token { get; set; }
    [Required] public string RefreshToken { get; set; }
}