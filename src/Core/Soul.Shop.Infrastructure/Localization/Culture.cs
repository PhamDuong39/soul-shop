using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Infrastructure.Localization;

public class Culture : EntityBaseWithTypedId<string>
{
    public Culture(string id)
    {
        Id = id;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public IList<Resource> Resources { get; set; }
}