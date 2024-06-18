using Shop.Module.Core.Entities;
using System.Threading.Tasks;

namespace Shop.Module.Core.Extensions;

public interface IWorkContext
{
    Task<User> GetCurrentUserAsync();

    Task<User> GetCurrentUserOrNullAsync();

    Task<User> GetCurrentOrThrowAsync();
    bool ValidateToken(int userId, string token, out int statusCode, string path = "");
}