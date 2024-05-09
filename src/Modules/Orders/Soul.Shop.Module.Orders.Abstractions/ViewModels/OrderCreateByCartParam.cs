using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateByCartParam
{
    [Required] public int ShippingUserAddressId { get; set; }
    [StringLength(450)]
    public string OrderNote { get; set; }
}