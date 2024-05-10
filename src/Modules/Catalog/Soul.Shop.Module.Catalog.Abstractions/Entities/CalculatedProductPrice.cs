namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class CalculatedProductPrice
{
    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public int PercentOfSaving { get; set; } // phần trăm giảm giá
}