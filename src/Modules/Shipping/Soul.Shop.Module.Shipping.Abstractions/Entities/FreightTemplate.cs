using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Shipping.Abstractions.Entities;

public class FreightTemplate : EntityBase
{
    public FreightTemplate()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [StringLength(450)] public string Name { get; set; }

    public string Note { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<PriceAndDestination> PriceAndDestinations { get; set; } = new List<PriceAndDestination>();
}