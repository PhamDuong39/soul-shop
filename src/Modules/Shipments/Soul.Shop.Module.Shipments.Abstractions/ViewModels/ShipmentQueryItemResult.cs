namespace Soul.Shop.Module.Shipments.Abstractions.ViewModels;

public class ShipmentQueryItemResult
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductMediaUrl { get; set; }

    public int OrderedQuantity { get; set; }

    public int ShippedQuantity { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}