﻿using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Inventory.Abstractions.Entities;

public class StockHistory : EntityBase
{
    public StockHistory()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; }

    public int AdjustedQuantity { get; set; }

    public int StockQuantity { get; set; }

    public string Note { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }
}