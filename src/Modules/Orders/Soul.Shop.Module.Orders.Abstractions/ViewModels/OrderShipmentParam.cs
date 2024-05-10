using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderShipmentParam
{
    [Required] public string TrackingNumber { get; set; }

    public decimal TotalWeight { get; set; }

    public string AdminComment { get; set; }

    public IList<OrderShipmentItemParam> Items { get; set; } = new List<OrderShipmentItemParam>();
}