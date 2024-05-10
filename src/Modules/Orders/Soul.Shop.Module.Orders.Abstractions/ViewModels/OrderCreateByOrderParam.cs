using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateByOrderParam
{
    [Required] public int ShippingUserAddressId { get; set; }

    [Required] public int OrderId { get; set; }

    [StringLength(450)] public string OrderNote { get; set; }
}