using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Soul.Shop.Module.Core.Extensions;

public class EFConfigProvider(Action<DbContextOptionsBuilder> optionsAction) : ConfigurationProvider
{
    private Action<DbContextOptionsBuilder> OptionsAction { get; } = optionsAction;

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<EFConfigurationDbContext>();
        OptionsAction(builder);

        using var dbContext = new EFConfigurationDbContext(builder.Options);
        Data = dbContext.AppSettings.ToDictionary(c => c.Id, c => c.Value)!;
    }
}