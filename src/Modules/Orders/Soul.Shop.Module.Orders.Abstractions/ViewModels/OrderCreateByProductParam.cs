using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateByProductParam
{
    [Required] public int ShippingUserAddressId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [StringLength(450)]
    public string OrderNote { get; set; }
}