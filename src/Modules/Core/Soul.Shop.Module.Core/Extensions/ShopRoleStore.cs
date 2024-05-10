using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Data;

namespace Soul.Shop.Module.Core.Extensions;

public class ShopRoleStore(ShopDbContext context)
    : RoleStore<Role, ShopDbContext, int, UserRole, IdentityRoleClaim<int>>(context);