namespace Soul.Shop.Module.Shipments.Abstractions.ViewModels;

public class ShipmentQueryResult
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string OrderNo { get; set; }
    //bug: should be OrderNumber
    // public Order Status OrderStatus { get; set; }
    //bug: should be OrderStatus?
    // public ShippingStatus? ShippingStatus { get; set; }

    public string TrackingNumber { get; set; }

    public decimal TotalWeight { get; set; }

    public DateTime? ShippedOn { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public string AdminComment { get; set; }

    public string CreatedBy { get; set; }

    public IList<ShipmentQueryItemResult> Items { get; set; } = new List<ShipmentQueryItemResult>();
}