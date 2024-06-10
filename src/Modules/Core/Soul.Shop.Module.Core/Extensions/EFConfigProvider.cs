using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Soul.Shop.Module.Core.Extensions;

public class EfConfigProvider(Action<DbContextOptionsBuilder> optionsAction) : ConfigurationProvider
{
    private Action<DbContextOptionsBuilder> OptionsAction { get; } = optionsAction;

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<EfConfigurationDbContext>();
        OptionsAction(builder);

        using var dbContext = new EfConfigurationDbContext(builder.Options);
        Data = dbContext.AppSettings.ToDictionary(c => c.Id, c => c.Value)!;
    }
}
