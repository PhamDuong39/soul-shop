using MediatR;
using Soul.Shop.Module.Core.Abstractions.Events;
using Soul.Shop.Module.Core.Abstractions.Extensions;
using Soul.Shop.Module.ShoppingCart.Abstractions.Services;

namespace Soul.Shop.Module.ShoppingCart.Handlers;

public class UserSignedInHandler(IWorkContext workContext, ICartService cartService)
    : INotificationHandler<UserSignedIn>
{
    public async Task Handle(UserSignedIn user, CancellationToken cancellationToken)
    {
        var guestUser = await workContext.GetCurrentUserAsync();
        await cartService.MigrateCart(guestUser.Id, user.UserId);
    }
}