using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class Role : IdentityRole<int>, IEntityWithTypedId<int>
{
    public IList<UserRole> Users { get; set; } = new List<UserRole>();
}