using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Orders.Abstractions.Entities;

public class OrderItem : EntityBase
{
    public OrderItem()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int OrderId { get; set; }

    [JsonIgnore] public Order Order { get; set; }

    public int ProductId { get; set; }

    [JsonIgnore] public Product Product { get; set; }

    public decimal ProductPrice { get; set; }

    public string ProductName { get; set; }

    public string ProductMediaUrl { get; set; }

    public int Quantity { get; set; }

    public int ShippedQuantity { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ItemAmount { get; set; }

    public decimal ItemWeight { get; set; }

    public string Note { get; set; }

    public int CreatedById { get; set; }

    [JsonIgnore] public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    [JsonIgnore] public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}