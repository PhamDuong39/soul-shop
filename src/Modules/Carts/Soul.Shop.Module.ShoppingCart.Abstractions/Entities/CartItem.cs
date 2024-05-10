using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.ShoppingCart.Abstractions.Entities;

public class CartItem : EntityBase
{
    public CartItem()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
        IsChecked = true;
    }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public int Quantity { get; set; }

    public int CartId { get; set; }

    public Cart Cart { get; set; }

    public bool IsChecked { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }
}