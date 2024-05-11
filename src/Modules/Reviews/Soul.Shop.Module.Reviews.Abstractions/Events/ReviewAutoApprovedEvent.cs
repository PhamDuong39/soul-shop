using MediatR;

namespace Soul.Shop.Module.Reviews.Abstractions.Events;

public class ReviewAutoApprovedEvent : INotification
{
    public int ReviewId { get; set; }
}