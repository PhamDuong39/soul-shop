using Microsoft.EntityFrameworkCore;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Extensions;

public class EfConfigurationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppSetting> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppSetting>().ToTable("Core_AppSetting");
    }
}
