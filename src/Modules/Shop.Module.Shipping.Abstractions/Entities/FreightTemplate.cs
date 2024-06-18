using System.ComponentModel.DataAnnotations;
using Shop.Infrastructure.Models;
using Shop.Module.Shipping.Entities;

namespace Shop.Module.Shipping.Abstractions.Entities;

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