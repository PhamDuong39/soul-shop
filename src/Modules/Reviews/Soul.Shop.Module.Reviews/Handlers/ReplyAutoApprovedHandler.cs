using MediatR;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Reviews.Abstractions.Entities;
using Soul.Shop.Module.Reviews.Abstractions.Events;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Handlers;

public class ReplyAutoApprovedHandler(IRepository<Reply> repository) : INotificationHandler<ReplyAutoApprovedEvent>
{
    public async Task Handle(ReplyAutoApprovedEvent notification, CancellationToken cancellationToken)
    {
        if (notification?.ReplyId > 0)
        {
            var reply = await repository.FirstOrDefaultAsync(notification.ReplyId);
            if (reply is { Status: ReplyStatus.Pending })
            {
                reply.Status = ReplyStatus.Approved;
                reply.UpdatedOn = DateTime.Now;
                await repository.SaveChangesAsync();
            }
        }
    }
}