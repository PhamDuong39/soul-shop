using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Shipping.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class Product : EntityBase
{
    public Product()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [ForeignKey("ParentProduct")] public int? ParentGroupedProductId { get; set; }

    [ForeignKey("ParentGroupedProductId")] public Product ParentProduct { get; set; } //danh mục cha

    public IList<Product> Childrens { get; set; } = new List<Product>();

    [Required] [StringLength(450)] public string Name { get; set; }

    [Required] [StringLength(450)] public string Slug { get; set; }

    public string MetaTitle { get; set; }

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public string ShortDescription { get; set; }

    public string Description { get; set; }

    public string Specification { get; set; }

    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? SpecialPrice { get; set; }

    public DateTime? SpecialPriceStart { get; set; } //giá khuyến mãi bắt đầu

    public DateTime? SpecialPriceEnd { get; set; } //giá khuyến mãi kết thúc

    public bool HasOptions { get; set; }

    public bool IsVisibleIndividually { get; set; } //có hiển thị riêng lẻ

    public bool IsFeatured { get; set; }

    public bool IsCallForPricing { get; set; }

    public bool IsAllowToOrder { get; set; }

    public bool StockTrackingIsEnabled { get; set; } //theo dõi kho hàng được bật

    [StringLength(450)] public string Sku { get; set; }

    [StringLength(450)] public string Gtin { get; set; }

    [StringLength(450)] public string NormalizedName { get; set; }

    public Media ThumbnailImage { get; set; }

    public int? ThumbnailImageId { get; set; }

    public int ReviewsCount { get; set; }

    public double? RatingAverage { get; set; }

    public int? BrandId { get; set; }

    public Brand Brand { get; set; }

    public string Barcode { get; set; } //mã vạch

    public int? ValidThru { get; set; }


    //public int? DefaultWarehouseId { get; set; }

    //public Warehouse DefaultWarehouse { get; set; }

    //public int StockId { get; set; }

    //public Stock Stock { get; set; }

    public int OrderMinimumQuantity { get; set; } //số lượng đặt hàng tối thiểu
    public int OrderMaximumQuantity { get; set; } //số lượng đặt hàng tối đa

    /// <summary>
    /// Gets or sets a value indicating whether to display stock availability
    /// </summary>
    public bool DisplayStockAvailability { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display stock quantity
    /// </summary>
    public bool DisplayStockQuantity { get; set; }

    public StockReduceStrategy StockReduceStrategy { get; set; }

    public bool NotReturnable { get; set; }

    public PublishType PublishType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedOn { get; set; }

    public DateTime? UnpublishedOn { get; set; }

    public string UnpublishedReason { get; set; }

    public bool IsShipEnabled { get; set; }


    public decimal Weight { get; set; }


    public decimal Length { get; set; }


    public decimal Width { get; set; }


    public decimal Height { get; set; }


    public bool IsFreeShipping { get; set; }

    /// <summary>
    /// Gets or sets the additional shipping charge
    /// </summary>
    public decimal AdditionalShippingCharge { get; set; }


    public int? FreightTemplateId { get; set; }


    public FreightTemplate FreightTemplate { get; set; }

    public int? UnitId { get; set; }

    public Unit Unit { get; set; }
    public string AdminRemark { get; set; }

    public int? DeliveryTime { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<ProductMedia> Medias { get; protected set; } = new List<ProductMedia>();

    public IList<ProductAttributeValue> AttributeValues { get; protected set; } = new List<ProductAttributeValue>();

    public IList<ProductOptionValue> OptionValues { get; protected set; } = new List<ProductOptionValue>();

    public IList<ProductCategory> Categories { get; protected set; } = new List<ProductCategory>();

    public IList<ProductPriceHistory> PriceHistories { get; protected set; } = new List<ProductPriceHistory>();

    public IList<ProductOptionCombination> OptionCombinations { get; protected set; } =
        new List<ProductOptionCombination>();

    //public IList<StockHistory> StockHistories { get; protected set; } = new List<StockHistory>();

    //public void AddStockHistory(StockHistory stockHistory)
    //{
    //    stockHistory.Product = this;
    //    StockHistories.Add(stockHistory);
    //}

    public void AddCategory(ProductCategory category)
    {
        category.Product = this;
        Categories.Add(category);
    }

    public void AddMedia(ProductMedia media)
    {
        media.Product = this;
        Medias.Add(media);
    }

    public void AddAttributeValue(ProductAttributeValue attributeValue)
    {
        attributeValue.Product = this;
        AttributeValues.Add(attributeValue);
    }

    public void AddOptionValue(ProductOptionValue optionValue)
    {
        optionValue.Product = this;
        OptionValues.Add(optionValue);
    }

    public void AddOptionCombination(ProductOptionCombination combination)
    {
        combination.Product = this;
        OptionCombinations.Add(combination);
    }

    public void AddPriceHistory(User loginUser)
    {
        var priceHistory = new ProductPriceHistory
        {
            CreatedBy = loginUser,
            UpdatedBy = loginUser,
            Product = this,
            Price = Price,
            OldPrice = OldPrice,
            SpecialPrice = SpecialPrice,
            SpecialPriceStart = SpecialPriceStart,
            SpecialPriceEnd = SpecialPriceEnd
        };
        PriceHistories.Add(priceHistory);
    }

    public Product Clone()
    {
        var product = new Product();
        product.Name = Name;
        product.MetaTitle = MetaTitle;
        product.MetaKeywords = MetaKeywords;
        product.MetaDescription = MetaDescription;
        product.ShortDescription = ShortDescription;
        product.Description = Description;
        product.Specification = Specification;
        product.Price = Price;
        product.OldPrice = OldPrice;
        product.SpecialPrice = SpecialPrice;
        product.SpecialPriceStart = SpecialPriceStart;
        product.SpecialPriceEnd = SpecialPriceEnd;
        product.HasOptions = HasOptions;
        product.IsVisibleIndividually = IsVisibleIndividually;
        product.IsFeatured = IsFeatured;
        product.IsAllowToOrder = IsAllowToOrder;
        product.IsCallForPricing = IsCallForPricing;
        product.BrandId = BrandId;
        product.StockTrackingIsEnabled = StockTrackingIsEnabled;
        product.Sku = Sku;
        product.Gtin = Gtin;
        product.NormalizedName = NormalizedName;
        product.DisplayOrder = DisplayOrder;
        product.Slug = Slug;

        product.IsPublished = IsPublished;
        product.PublishedOn = PublishedOn;
        product.Barcode = Barcode;
        product.DeliveryTime = DeliveryTime;
        product.ValidThru = ValidThru;
        //product.DefaultWarehouseId = DefaultWarehouseId;
        product.PublishType = PublishType;
        product.OrderMaximumQuantity = OrderMaximumQuantity;
        product.OrderMinimumQuantity = OrderMinimumQuantity;
        product.DisplayStockAvailability = DisplayStockAvailability;
        product.DisplayStockQuantity = DisplayStockQuantity;
        product.NotReturnable = NotReturnable;
        product.StockReduceStrategy = StockReduceStrategy;
        product.UnpublishedOn = UnpublishedOn;
        product.UnpublishedReason = UnpublishedReason;

        product.AdditionalShippingCharge = AdditionalShippingCharge;
        product.AdminRemark = AdminRemark;
        product.FreightTemplateId = FreightTemplateId;
        product.Height = Height;
        product.IsFreeShipping = IsFreeShipping;
        product.IsShipEnabled = IsShipEnabled;
        product.Length = Length;
        product.Weight = Weight;
        product.Width = Width;
        product.UnitId = UnitId;


        foreach (var attribute in AttributeValues)
            product.AddAttributeValue(new ProductAttributeValue
            {
                AttributeId = attribute.AttributeId,
                Value = attribute.Value
            });

        foreach (var category in Categories)
            product.AddCategory(new ProductCategory
            {
                CategoryId = category.CategoryId
            });

        return product;
    }
}