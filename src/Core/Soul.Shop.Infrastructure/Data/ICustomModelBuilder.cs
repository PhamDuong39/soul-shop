using Microsoft.EntityFrameworkCore;

namespace Soul.Shop.Infrastructure.Data;

public interface ICustomModelBuilder
{
    void Build(ModelBuilder modelBuilder);
}