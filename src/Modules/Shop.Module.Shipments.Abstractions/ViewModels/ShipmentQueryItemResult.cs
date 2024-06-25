using System;

namespace Shop.Module.Shipments.ViewModels;

public class ShipmentQueryItemResult
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// Tên sản phẩm (ảnh chụp nhanh)
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Hình ảnh sản phẩm (ảnh chụp nhanh)
    /// </summary>
    public string ProductMediaUrl { get; set; }

    /// <summary>
    /// Số lượng đặt hàng
    /// </summary>
    public int OrderedQuantity { get; set; }

    /// <summary>
    /// Số lượng vận chuyển
    /// </summary>
    public int ShippedQuantity { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
