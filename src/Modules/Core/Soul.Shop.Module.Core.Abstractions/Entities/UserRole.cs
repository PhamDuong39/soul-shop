using Microsoft.AspNetCore.Identity;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class UserRole : IdentityUserRole<int>
{
    public override int UserId { get; set; }

    public User User { get; set; }

    public override int RoleId { get; set; }

    public Role Role { get; set; }
}