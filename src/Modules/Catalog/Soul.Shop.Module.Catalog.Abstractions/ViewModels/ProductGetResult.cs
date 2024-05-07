using Soul.Shop.Module.Catalog.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductGetResult
{
    public int Id { get; set; }

    public int ParentGroupedProductId { get; set; }

    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? SpecialPrice { get; set; }

    public DateTime? SpecialPriceStart { get; set; }

    public DateTime? SpecialPriceEnd { get; set; }

    public bool IsCallForPricing { get; set; }

    public bool IsAllowToOrder { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string MetaTitle { get; set; }

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public string Sku { get; set; }

    public string Gtin { get; set; }

    public string ShortDescription { get; set; }

    public string Description { get; set; }

    public string Specification { get; set; }

    public bool IsPublished { get; set; }

    public bool IsFeatured { get; set; }

    public bool StockTrackingIsEnabled { get; set; }

    public IList<ProductGetAttributeResult> Attributes { get; set; } = new List<ProductGetAttributeResult>();

    public IList<ProductGetOptionResult> Options { get; set; } = new List<ProductGetOptionResult>();

    public IList<ProductGetVariationResult> Variations { get; set; } = new List<ProductGetVariationResult>();

    public IList<ProductGetMediaResult> ProductImages { get; set; } = new List<ProductGetMediaResult>();

    public IList<ProductGetStockResult> Stocks { get; set; } = new List<ProductGetStockResult>();

    public IList<int> CategoryIds { get; set; } = new List<int>();

    public IList<int> MediaIds { get; set; } = new List<int>();

    public int? MediaId { get; set; }

    public string MediaUrl { get; set; }

    public int? BrandId { get; set; }

    public DateTime? PublishedOn { get; set; }

    public string Barcode { get; set; }

    public int? ValidThru { get; set; }

    public int? DeliveryTime { get; set; }

    public IList<int> WarehouseIds { get; set; } = new List<int>();

    public int OrderMinimumQuantity { get; set; }

    public int OrderMaximumQuantity { get; set; }

    public bool DisplayStockAvailability { get; set; }

    public bool DisplayStockQuantity { get; set; }

    public StockReduceStrategy StockReduceStrategy { get; set; }

    public bool NotReturnable { get; set; }

    public PublishType PublishType { get; set; }

    public DateTime? UnpublishedOn { get; set; }

    public string UnpublishedReason { get; set; }

    public int StockQuantity { get; set; }

    public bool IsVisibleIndividually { get; set; }

    public bool IsShipEnabled { get; set; }

    public decimal Weight { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public bool IsFreeShipping { get; set; }

    public decimal AdditionalShippingCharge { get; set; }

    public int? FreightTemplateId { get; set; }

    public string AdminRemark { get; set; }

    public int? UnitId { get; set; }
}