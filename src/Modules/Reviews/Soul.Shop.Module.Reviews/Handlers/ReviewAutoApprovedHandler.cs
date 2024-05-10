using MediatR;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Reviews.Abstractions.Entities;
using Soul.Shop.Module.Reviews.Abstractions.Events;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Handlers;

public class ReviewAutoApprovedHandler(IRepository<Review> repository) : INotificationHandler<ReviewAutoApprovedEvent>
{
    public async Task Handle(ReviewAutoApprovedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ReviewId > 0)
        {
            var review = await repository.FirstOrDefaultAsync(notification.ReviewId);
            if (review is { Status: ReviewStatus.Pending })
            {
                review.Status = ReviewStatus.Approved;
                review.UpdatedOn = DateTime.Now;
                await repository.SaveChangesAsync();
            }
        }
    }
}