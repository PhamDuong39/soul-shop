using MediatR;

namespace Soul.Shop.Module.Reviews.Abstractions.Events;

public class ReplyAutoApprovedEvent : INotification
{
    public int ReplyId { get; set; }
}