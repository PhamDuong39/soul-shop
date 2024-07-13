using Shop.Module.Catalog.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels;

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

    /// <summary>
    /// Product barcode
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Product validity period. Unit: day. Calculated from the release/listing time. If expired, the release/listing will be automatically cancelled. When publishing, the listing time is calculated.
    /// Function can be provided. When the product expires/is about to expire, it will be automatically released/listed to recalculate the listing/listing time.
    /// </summary>
    public int? ValidThru { get; set; }

    /// <summary>
    /// Stocking period. Value range: 1-60; unit: day.
    /// </summary>
    public int? DeliveryTime { get; set; }

    public IList<int> WarehouseIds { get; set; } = new List<int>();

    /// <summary>
    /// Gets or sets the order minimum quantity
    /// </summary>
    public int OrderMinimumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the order maximum quantity
    /// </summary>
    public int OrderMaximumQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display stock availability
    /// </summary>
    public bool DisplayStockAvailability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display stock quantity
    /// </summary>
    public bool DisplayStockQuantity { get; set; }

    /// <summary>
    /// There are two types of inventory deduction strategies: place_order_withhold and payment_success_deduct.
    /// </summary>
    public StockReduceStrategy StockReduceStrategy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is returnable (a customer is allowed to submit return request with this product)
    /// </summary>
    public bool NotReturnable { get; set; }

    public PublishType PublishType { get; set; }

    public DateTime? UnpublishedOn { get; set; }

    /// <summary>
    /// Reason for cancelling the publication
    /// </summary>
    public string UnpublishedReason { get; set; }

    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the values indicating whether this product is visible in catalog or search results.
    /// It's used when this product is associated to some "grouped" one
    /// This way associated products could be accessed/added/etc only from a grouped product details page
    /// </summary>
    public bool IsVisibleIndividually { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is ship enabled
    /// </summary>
    public bool IsShipEnabled { get; set; }

    /// <summary>
    /// Gets or sets the weight
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Gets or sets the length
    /// </summary>
    public decimal Length { get; set; }

    /// <summary>
    /// Gets or sets the width
    /// </summary>
    public decimal Width { get; set; }

    /// <summary>
    /// Gets or sets the height
    /// </summary>
    public decimal Height { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is free shipping
    /// </summary>
    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// Gets or sets the additional shipping charge
    /// </summary>
    public decimal AdditionalShippingCharge { get; set; }

    /// <summary>
    /// Freight Template Id
    /// </summary>
    public int? FreightTemplateId { get; set; }

    /// <summary>
    /// Administrator Notes
    /// </summary>
    public string AdminRemark { get; set; }

    public int? UnitId { get; set; }
}
