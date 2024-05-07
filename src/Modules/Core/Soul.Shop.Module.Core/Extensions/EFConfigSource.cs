using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Soul.Shop.Module.Core.Extensions;

public class EFConfigSource(Action<DbContextOptionsBuilder> optionsAction) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EFConfigProvider(optionsAction);
    }
}