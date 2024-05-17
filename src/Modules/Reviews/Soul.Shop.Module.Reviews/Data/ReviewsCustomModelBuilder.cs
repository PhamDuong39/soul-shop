using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Reviews.Abstractions.Data;
using Soul.Shop.Module.Reviews.Abstractions.Entities;

namespace Soul.Shop.Module.Reviews.Data;

public class ReviewsCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        const string module = "Reviews";

        modelBuilder.Entity<Reply>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Support>().HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<EntityType>().HasData(
            new EntityType()
            {
                Id = (int)EntityTypeWithId.Review, Name = EntityTypeWithId.Review.GetDisplayName(), Module = module,
                IsMenuable = false
            },
            new EntityType()
            {
                Id = (int)EntityTypeWithId.Reply, Name = EntityTypeWithId.Reply.GetDisplayName(), Module = module,
                IsMenuable = false
            }
        );

        modelBuilder.Entity<AppSetting>().HasData(
            new AppSetting(ReviewKeys.IsReviewAutoApproved)
            {
                Module = module, IsVisibleInCommonSettingPage = true, Value = "false", Type = typeof(bool).FullName,
                Note = "Enable automatic comment moderation"
            },
            new AppSetting(ReviewKeys.IsReplyAutoApproved)
            {
                Module = module, IsVisibleInCommonSettingPage = true, Value = "true", Type = typeof(bool).FullName,
                Note = "Enable automatic review of replies"
            }
        );
    }
}