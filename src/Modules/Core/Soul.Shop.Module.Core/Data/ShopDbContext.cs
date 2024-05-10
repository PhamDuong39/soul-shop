using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Data;

public class ShopDbContext(DbContextOptions options)
    : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, UserLogin,
        IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ValidateEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ValidateEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Enable sensitive data logging
        optionsBuilder.EnableSensitiveDataLogging();

        // Add your other configuration options here
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var typeToRegisters = new List<Type>();
        foreach (var module in GlobalConfiguration.Modules)
            typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));

        RegisterEntities(modelBuilder, typeToRegisters);

        RegisterConvention(modelBuilder);

        base.OnModelCreating(modelBuilder);

        RegisterCustomMappings(modelBuilder, typeToRegisters);
    }

    private void ValidateEntities()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var entity in modifiedEntries)
            if (entity.Entity is ValidatableObject validatableObject)
            {
                var validationResults = validatableObject.Validate();
                if (validationResults.Any()) throw new ValidationException(entity.Entity.GetType(), validationResults);
            }
    }

    private static void RegisterConvention(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
            if (entity.ClrType.Namespace != null)
            {
                var nameParts = entity.ClrType.Namespace.Split('.');
                var tableName = string.Concat(nameParts[2], "_", entity.ClrType.Name);
                modelBuilder.Entity(entity.Name).ToTable(tableName);
            }

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
    }

    private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
    {
        var entityTypes = typeToRegisters.Where(x =>
            x.GetTypeInfo().IsSubclassOf(typeof(EntityBase)) && !x.GetTypeInfo().IsAbstract);
        foreach (var type in entityTypes) modelBuilder.Entity(type);
    }

    private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
    {
        var customModelBuilderTypes = typeToRegisters.Where(x => typeof(ICustomModelBuilder).IsAssignableFrom(x));
        foreach (var builderType in customModelBuilderTypes)
            if (builderType != typeof(ICustomModelBuilder))
            {
                var builder = (ICustomModelBuilder)Activator.CreateInstance(builderType)!;
                builder.Build(modelBuilder);
            }
    }
}