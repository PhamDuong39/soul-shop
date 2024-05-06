using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class UserAddressCreateParam
{
    [MaxLength(20)]
    [Required(ErrorMessage = "ContactName is required")]
    public string ContactName { get; set; }

    [MaxLength(20)]
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }

    [MaxLength(200)]
    [Required(ErrorMessage = "AddressLine1 is required")]
    public string AddressLine1 { get; set; }

    [Required(ErrorMessage = "StateOrProvinceId is required")]
    public int StateOrProvinceId { get; set; }

    [Required(ErrorMessage = "CityId is required")]
    public int CityId { get; set; }

    public int? DistrictId { get; set; }

    public bool IsDefault { get; set; }
}