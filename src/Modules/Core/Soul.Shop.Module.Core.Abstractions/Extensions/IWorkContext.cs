using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Extensions;

public interface IWorkContext
{
    Task<User> GetCurrentUserAsync();

    Task<User> GetCurrentUserOrNullAsync();

    Task<User> GetCurrentOrThrowAsync();
    bool ValidateToken(int userId, string token, out int statusCode, string path = "");
}