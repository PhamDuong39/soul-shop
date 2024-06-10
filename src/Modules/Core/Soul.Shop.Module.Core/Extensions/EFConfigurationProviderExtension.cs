using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Soul.Shop.Module.Core.Extensions;

public static class EfConfigurationProviderExtension
{
    public static IConfigurationBuilder AddEntityFrameworkConfig(this IConfigurationBuilder builder,
        Action<DbContextOptionsBuilder> setup)
    {
        return builder.Add(new EFConfigSource(setup));
    }
}
