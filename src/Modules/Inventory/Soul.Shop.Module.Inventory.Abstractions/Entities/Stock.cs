using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Entities;

namespace Soul.Shop.Module.Inventory.Abstractions.Entities;

public class Stock : EntityBase
{
    public Stock()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int StockQuantity { get; set; }

    public int LockedStockQuantity { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsEnabled { get; set; }
    [StringLength(450)] public string Note { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}