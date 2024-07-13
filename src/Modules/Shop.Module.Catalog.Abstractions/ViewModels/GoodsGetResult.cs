using System;
using System.Collections.Generic;

namespace Shop.Module.Catalog.ViewModels;

public class GoodsGetResult
{
    public int Id { get; set; }

    public int ParentGroupedProductId { get; set; }

    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? SpecialPrice { get; set; }

    public DateTime? SpecialPriceStart { get; set; }

    public DateTime? SpecialPriceEnd { get; set; }

    public bool IsAllowToOrder { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string Sku { get; set; }

    public string Gtin { get; set; }

    public string ShortDescription { get; set; }

    public string Description { get; set; }

    public string Specification { get; set; }

    public bool IsPublished { get; set; }

    public bool IsFeatured { get; set; }

    public int? MediaId { get; set; }

    public string MediaUrl { get; set; }

    public int? BrandId { get; set; }

    public string BrandName { get; set; }

    /// <summary>
    /// Product barcode
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Stocking period. Value range: 1-60; unit: day.
    /// </summary>
    public int? DeliveryTime { get; set; }

    /// <summary>
    /// Gets or sets the order minimum quantity
    /// </summary>
    public int OrderMinimumQuantity { get; set; }

    /// <summary>
    /// Gets or sets the order maximum quantity
    /// </summary>
    public int OrderMaximumQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this product is returnable (a customer is allowed to submit return request with this product)
    /// </summary>
    public bool NotReturnable { get; set; }

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

    public int? UnitId { get; set; }

    public string UnitName { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? PublishedOn { get; set; }

    public bool StockTrackingIsEnabled { get; set; }

    public IList<int> CategoryIds { get; set; } = new List<int>();

    public IList<GoodsGetAttributeResult> Attributes { get; set; } = new List<GoodsGetAttributeResult>();

    public IList<ProductGetOptionResult> Options { get; set; } = new List<ProductGetOptionResult>();

    public IList<ProductGetVariationResult> Variations { get; set; } = new List<ProductGetVariationResult>();

    public IList<ProductGetMediaResult> ProductImages { get; set; } = new List<ProductGetMediaResult>();

    public IList<GoodsGetIssueResult> Issues { get; set; } = new List<GoodsGetIssueResult>()
    {
        new() { Id = 1, Answer = "1. Within 30 days from the date of receipt of the goods, customers can apply for worry-free returns. The refund will be returned to the original route. Different banks have different processing times.", ProductId = 0, Question = "How to apply for a return?" },
        new() { Id = 1, Answer = "1.If you need to issue a general invoice，Please select when placing an order“I want to issue an invoice”And fill in the relevant information（APP is limited to 2.4.0 and above", ProductId = 0, Question = "How to issue an invoice？" },
        new()
        {
            Id = 1, Answer = "Yanxuan uses SF Express by default for delivery（Some products use other couriers），The delivery range covers most areas of the country（Except Hong Kong, Macau and Taiwan", ProductId = 0, Question = "What courier to use for delivery？"
        }
    };
}

public class GoodsGetAttributeResult
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class GoodsGetIssueResult
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int DisplayOrder { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
}
