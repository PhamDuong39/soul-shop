using MediatR;
using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Catalog.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Events;
using Soul.Shop.Module.Core.Abstractions.Extensions;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Handlers;

public class EntityViewedHandler(
    IRepository<ProductRecentlyViewed> recentlyViewedProductRepository,
    IWorkContext workcontext)
    : INotificationHandler<EntityViewed>
{
    private readonly IWorkContext _workContext = workcontext;

    public async Task Handle(EntityViewed notification, CancellationToken cancellationToken)
    {
        if (notification.EntityTypeWithId == EntityTypeWithId.Product)
        {
            var model = await recentlyViewedProductRepository.Query()
                .FirstOrDefaultAsync(x => x.ProductId == notification.EntityId && x.CustomerId == notification.UserId,
                    cancellationToken: cancellationToken);

            if (model == null)
            {
                model = new ProductRecentlyViewed
                {
                    CustomerId = notification.UserId,
                    ProductId = notification.EntityId
                };
                recentlyViewedProductRepository.Add(model);
            }

            model.ViewedCount++;
            model.LatestViewedOn = DateTime.Now;
            await recentlyViewedProductRepository.SaveChangesAsync();
        }
    }
}