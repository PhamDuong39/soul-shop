using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Shipments.Abstractions.Entities;

public class Shipment : EntityBase
{
    public Shipment()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int OrderId { get; set; }
    //bug: should be Order
    // public Order Order { get; set; }

    public decimal TotalWeight { get; set; }

    [StringLength(450)] public string TrackingNumber { get; set; }

    public DateTime? ShippedOn { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public string AdminComment { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
}