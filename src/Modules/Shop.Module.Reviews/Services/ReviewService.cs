using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.MQ;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Models;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Support> _supportRepository;
    private readonly IWorkContext _workContext;
    private readonly IRepository<Order> _orderRepository;
    private readonly IMQService _mqService;
    private readonly IAppSettingService _appSettingService;
    private readonly IRepository<User> _userRepository;

    public ReviewService(
        IRepository<Review> reviewRepository,
        IRepository<Support> supportRepository,
        IWorkContext workContext,
        IRepository<Order> orderRepository,
        IMQService mqService,
        IAppSettingService appSettingService,
        IRepository<User> userRepository)
    {
        _reviewRepository = reviewRepository;
        _supportRepository = supportRepository;
        _workContext = workContext;
        _orderRepository = orderRepository;
        _mqService = mqService;
        _appSettingService = appSettingService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Khen ngợi tự động
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="entityTypeId"></param>
    /// <param name="sourceId"></param>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    public async Task ReviewAutoGood(int entityId, EntityTypeWithId entityTypeId, int? sourceId,
        ReviewSourceType? sourceType)
    {
        var systemUserId = (int)UserWithId.System;
        User user = null;
        if (sourceType != null && sourceId != null)
            if (sourceType == ReviewSourceType.Order && entityTypeId == EntityTypeWithId.Product)
            {
                var anyProduct = _orderRepository.Query().Any(c =>
                    c.Id == sourceId.Value && c.OrderItems.Any(x => x.ProductId == entityId));
                if (!anyProduct) return;
                var order = await _orderRepository.Query().FirstOrDefaultAsync(c => c.Id == sourceId.Value);
                if (order == null)
                    return;
                if (order.OrderStatus != OrderStatus.Complete)
                    return;

                user = await _userRepository.FirstOrDefaultAsync(order.CustomerId);
            }

        // a user
        // comments on a given order and product can only be made once
        // comments not related to the order. You can only comment on a product once.
        var any = await _reviewRepository.Query().AnyAsync(c =>
            c.EntityTypeId == (int)entityTypeId && c.EntityId == entityId && c.SourceId == sourceId &&
            c.SourceType == sourceType);
        if (any) return;

        var review = new Review
        {
            Rating = 5,
            Comment = "Default positive",
            EntityId = entityId,
            EntityTypeId = (int)entityTypeId,
            SourceId = sourceId,
            SourceType = sourceType,
            UserId = user?.Id ?? systemUserId,
            IsSystem = true,
            IsAnonymous = true,
            Status = ReviewStatus.Approved,
            ReviewerName = user == null ? "***" : $"{user.FullName.First()}***{user.FullName.Last()}"
        };
        _reviewRepository.Add(review);
        await _reviewRepository.SaveChangesAsync();
    }
}
