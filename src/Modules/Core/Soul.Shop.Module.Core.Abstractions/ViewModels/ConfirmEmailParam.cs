using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class ConfirmEmailParam
{
    [Required] public int UserId { get; set; }

    [Required] public string Code { get; set; }
}