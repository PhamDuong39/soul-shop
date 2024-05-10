using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Feedbacks.Abstractions.Entities;

namespace Soul.Shop.Module.Feedbacks.Data;

public class FeedbackCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>().HasQueryFilter(c => !c.IsDeleted);
    }
}