using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Data;

namespace Soul.Shop.Module.Core.Extensions;

public class ShopUserStore(ShopDbContext context, IdentityErrorDescriber describer)
    : UserStore<User, Role, ShopDbContext, int, IdentityUserClaim<int>, UserRole, UserLogin,
        IdentityUserToken<int>, IdentityRoleClaim<int>>(context, describer);