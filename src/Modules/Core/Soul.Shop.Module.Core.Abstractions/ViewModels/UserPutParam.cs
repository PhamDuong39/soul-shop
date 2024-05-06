using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class UserPutParam
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(20, ErrorMessage = "FullName must be between 1 and 20 characters", MinimumLength = 1)]
    public string FullName { get; set; }

    public int? MediaId { get; set; }

    //public string AdminRemark { get; set; }
}