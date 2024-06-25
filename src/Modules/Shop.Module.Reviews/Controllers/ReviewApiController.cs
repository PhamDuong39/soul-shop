using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.MQ;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Models;
using Shop.Module.Reviews.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Events;
using Shop.Module.Reviews.Models;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers;

/// <summary>
/// Bộ điều khiển API nhận xét, chịu trách nhiệm xử lý các hoạt động liên quan đến nhận xét.
/// </summary>
[Route("api/reviews")]
[Authorize()]
public class ReviewApiController : ControllerBase
{
    private readonly EntityTypeWithId[] entityTypeIds = new EntityTypeWithId[] { EntityTypeWithId.Product };
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Support> _supportRepository;
    private readonly IWorkContext _workContext;
    private readonly IRepository<Order> _orderRepository;
    private readonly IMQService _mqService;
    private readonly IAppSettingService _appSettingService;
    private readonly IRepository<Reply> _replyRepository;

    public ReviewApiController(
        IRepository<Review> reviewRepository,
        IRepository<Support> supportRepository,
        IWorkContext workContext,
        IRepository<Order> orderRepository,
        IMQService mqService,
        IAppSettingService appSettingService,
        IRepository<Reply> replyRepository)
    {
        _reviewRepository = reviewRepository;
        _supportRepository = supportRepository;
        _workContext = workContext;
        _orderRepository = orderRepository;
        _mqService = mqService;
        _appSettingService = appSettingService;
        _replyRepository = replyRepository;
    }

    /// <summary>
    /// thêm bình luận
    /// </summary>
    /// <param name="param">Nhận xét các tham số được thêm vào. </param>
    /// <returns>Kết quả của thao tác thêm chú thích. </return>
    [HttpPost]
    public async Task<Result> AddReview([FromBody] ReviewAddParam param)
    {
        var user = await _workContext.GetCurrentOrThrowAsync();
        var anyType = entityTypeIds.Any(c => c == param.EntityTypeId);
        if (!anyType) throw new Exception("Ngoại lệ tham số");

        if (param.SourceType == null && param.SourceId != null)
            throw new Exception("Loại nguồn bình luận không bình thường");
        else if (param.SourceType != null && param.SourceId != null)
            if (param.SourceType == ReviewSourceType.Order && param.EntityTypeId == EntityTypeWithId.Product)
            {
                var anyProduct = _orderRepository.Query().Any(c =>
                    c.Id == param.SourceId.Value && c.OrderItems.Any(x => x.ProductId == param.EntityId));
                if (!anyProduct) throw new Exception("Sản phẩm được đánh giá không tồn tại");
                var order = await _orderRepository.Query().FirstOrDefaultAsync(c => c.Id == param.SourceId);
                if (order == null)
                    throw new Exception("Đơn hàng không tồn tại");
                if (order.OrderStatus != OrderStatus.Complete)
                    throw new Exception("Đơn hàng chưa được hoàn thành và không thể đánh giá được.");
            }

        // một người dùng bình luận về một đơn hàng và một sản phẩm nhất định chỉ có thể được thực hiện một lần
        // bình luận không liên quan đến đơn hàng. Bạn chỉ có thể bình luận về một sản phẩm một lần.
        var any = await _reviewRepository.Query()
            .AnyAsync(c =>
                c.UserId == user.Id && c.EntityTypeId == (int)param.EntityTypeId && c.EntityId == param.EntityId &&
                c.SourceId == param.SourceId && c.SourceType == param.SourceType);
        if (any)
            throw new Exception("Bạn đã bình luận");

        var review = new Review
        {
            Rating = param.Rating,
            Title = param.Title,
            Comment = param.Comment,
            EntityId = param.EntityId,
            EntityTypeId = (int)param.EntityTypeId,
            IsAnonymous = param.IsAnonymous,
            UserId = user.Id,
            ReviewerName = param.IsAnonymous ? $"{user.FullName.First()}***{user.FullName.Last()}" : user.FullName,
            SourceId = param.SourceId,
            SourceType = param.SourceType
        };
        if (param?.MediaIds.Count > 0)
        {
            var mediaIds = param.MediaIds.Distinct();
            var i = 0;
            foreach (var mediaId in mediaIds)
            {
                review.Medias.Add(new ReviewMedia()
                {
                    DisplayOrder = i,
                    MediaId = mediaId,
                    Review = review
                });
                i++;
            }
        }

        _reviewRepository.Add(review);
        _reviewRepository.SaveChanges();

        var isAuto = await _appSettingService.Get<bool>(ReviewKeys.IsReviewAutoApproved);
        if (isAuto)
            await _mqService.Send(QueueKeys.ReviewAutoApproved, new ReviewAutoApprovedEvent()
            {
                ReviewId = review.Id
            });
        return Result.Ok();
    }


    /// <summary>
    /// Lấy thông tin đánh giá của một thực thể cụ thể, chẳng hạn như tổng số đánh giá và số lượng đánh giá cho mỗi xếp hạng sao.
    /// </summary>
    /// <param name="param">Tham số cho truy vấn thông tin bình luận. </param>
    /// <returns>Thông tin nhận xét của các thực thể cụ thể. </return>
    [HttpPost("info")]
    [AllowAnonymous]
    public async Task<Result> Info([FromBody] ReviewInfoParam param)
    {
        var any = entityTypeIds.Any(c => c == param.EntityTypeId);
        if (!any)
            throw new Exception("Thông số không được hỗ trợ");

        var query = _reviewRepository.Query()
            .Where(c => c.EntityId == param.EntityId && c.EntityTypeId == (int)param.EntityTypeId &&
                        c.Status == ReviewStatus.Approved);
        var groupByRating = await query.GroupBy(c => c.Rating)
            .Select(c => new ReviewGroupByRating
            {
                Rating = c.Key,
                Count = c.Count()
            }).ToListAsync();
        var mediasCount = await query.Where(c => c.Medias.Any()).CountAsync();
        var result = new ReviewInfoResult()
        {
            MediasCount = mediasCount,
            ReviewsCount = groupByRating.Sum(c => c.Count),
            Rating1Count = groupByRating.FirstOrDefault(c => c.Rating == 1)?.Count ?? 0,
            Rating2Count = groupByRating.FirstOrDefault(c => c.Rating == 2)?.Count ?? 0,
            Rating3Count = groupByRating.FirstOrDefault(c => c.Rating == 3)?.Count ?? 0,
            Rating4Count = groupByRating.FirstOrDefault(c => c.Rating == 4)?.Count ?? 0,
            Rating5Count = groupByRating.FirstOrDefault(c => c.Rating == 5)?.Count ?? 0
        };
        return Result.Ok(result);
    }

    /// <summary>
    /// Liệt kê các chú thích cho một thực thể cụ thể.
    /// </summary>
    /// <param name="param">Tham số cho truy vấn danh sách nhận xét. </param>
    /// <returns>Danh sách các nhận xét cho một thực thể cụ thể. </return>
    [HttpPost("list")]
    [AllowAnonymous]
    public async Task<Result> List([FromBody] ReviewListQueryParam param)
    {
        var any = entityTypeIds.Any(c => c == param.EntityTypeId);
        if (!any)
            throw new Exception("Thông số không được hỗ trợ");

        var query = _reviewRepository.Query()
            .Where(c => c.Status == ReviewStatus.Approved && c.EntityId == param.EntityId &&
                        c.EntityTypeId == (int)param.EntityTypeId);

        var result = await query
            .Include(c => c.Medias).ThenInclude(c => c.Media)
            .Include(c => c.Replies)
            .Include(c => c.User)
            .OrderByDescending(c => c.SupportCount).ThenByDescending(c => c.Rating).ThenByDescending(c => c.Id)
            .Select(c => new ReviewListResult
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedOn = c.CreatedOn,
                Rating = c.Rating,
                Title = c.Title,
                ReviewerName = c.ReviewerName,
                SupportCount = c.SupportCount,
                Avatar = c.User.AvatarUrl,
                ReplieCount = c.Replies.Where(x => x.Status == ReplyStatus.Approved).Count(),
                MediaUrls = c.Medias.OrderBy(x => x.DisplayOrder).Select(x => x.Media.Url)
                //Replies = c.Replies.Where(x => x.Status == ReplyStatus.Approved && x.ParentId == null)
                //.OrderByDescending(x => x.SupportCount).ThenByDescending(x => x.Id)
                //.Take(2).Select(x => new ReplyListResult()
                //{
                //    Id = x.Id,
                //    Comment = x.Comment,
                //    ReplierName = x.ReplierName,
                //    CreatedOn = x.CreatedOn,
                //    SupportCount = x.SupportCount
                //})
            }).Take(param.Take).ToListAsync();

        // bug todo Để được tối ưu hóa
        result.ForEach(c =>
        {
            if (c.ReplieCount > 0)
                c.Replies = _replyRepository.Query(x =>
                        x.ReviewId == c.Id && x.Status == ReplyStatus.Approved && x.ParentId == null)
                    .OrderByDescending(x => x.SupportCount).ThenByDescending(x => x.Id)
                    .Take(2).Select(x => new ReplyListResult()
                    {
                        Id = x.Id,
                        Comment = x.Comment,
                        ReplierName = x.ReplierName,
                        CreatedOn = x.CreatedOn,
                        SupportCount = x.SupportCount
                    }).ToList();
        });

        return Result.Ok(result);
    }

    /// <summary>
    /// Lấy danh sách bình luận trong phân trang.
    /// </summary>
    /// <param name="param">Tham số truy vấn phân trang. </param>
    /// <returns> Danh sách bình luận được đánh số trang. </return>
    [HttpPost("grid")]
    [AllowAnonymous]
    public async Task<Result<StandardTableResult<ReviewListResult>>> Grid(
        [FromBody] StandardTableParam<ReviewQueryParam> param)
    {
        var search = param?.Search;
        if (search == null)
            throw new ArgumentNullException("Ngoại lệ tham số");

        var any = entityTypeIds.Any(c => c == search.EntityTypeId);
        if (!any)
            throw new Exception("Thông số không được hỗ trợ");

        var query = _reviewRepository.Query()
            .Where(c => c.Status == ReviewStatus.Approved && c.EntityId == search.EntityId &&
                        c.EntityTypeId == (int)search.EntityTypeId);

        if (search.IsMedia.HasValue && search.IsMedia.Value)
            query = query.Where(c => c.Medias.Any());
        if (search.RatingLevel.HasValue)
            switch (search.RatingLevel.Value)
            {
                case RatingLevel.Bad:
                    query = query.Where(c => c.Rating == 1);
                    break;

                case RatingLevel.Medium:
                    query = query.Where(c => c.Rating > 1 && c.Rating < 5);
                    break;

                case RatingLevel.Positive:
                default:
                    query = query.Where(c => c.Rating == 5);
                    break;
            }

        var result = await query
            .Include(c => c.Medias).ThenInclude(c => c.Media)
            .Include(c => c.Replies)
            .Include(c => c.User)
            .ToStandardTableResult(param, c => new ReviewListResult
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedOn = c.CreatedOn,
                Rating = c.Rating,
                Title = c.Title,
                ReviewerName = c.ReviewerName,
                SupportCount = c.SupportCount,
                Avatar = c.User.AvatarUrl,
                ReplieCount = c.Replies.Where(x => x.Status == ReplyStatus.Approved).Count(),
                MediaUrls = c.Medias.OrderBy(x => x.DisplayOrder).Select(x => x.Media.Url)
                //Replies = c.Replies.Where(x => x.Status == ReplyStatus.Approved && x.ParentId == null)
                //.OrderByDescending(x => x.SupportCount).ThenByDescending(x => x.Id)
                //.Take(2).Select(x => new ReplyListResult()
                //{
                //    Id = x.Id,
                //    Comment = x.Comment,
                //    ReplierName = x.ReplierName,
                //    CreatedOn = x.CreatedOn,
                //    SupportCount = x.SupportCount
                //})
            });

        if (result?.List?.Count() > 0)
            // bug todo Để được tối ưu hóa
            result.List.ToList().ForEach(c =>
            {
                if (c.ReplieCount > 0)
                    c.Replies = _replyRepository.Query(x =>
                            x.ReviewId == c.Id && x.Status == ReplyStatus.Approved && x.ParentId == null)
                        .OrderByDescending(x => x.SupportCount).ThenByDescending(x => x.Id)
                        .Take(2).Select(x => new ReplyListResult()
                        {
                            Id = x.Id,
                            Comment = x.Comment,
                            ReplierName = x.ReplierName,
                            CreatedOn = x.CreatedOn,
                            SupportCount = x.SupportCount
                        }).ToList();
            });

        return Result.Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết cho một bình luận cụ thể.
    /// </summary>
    /// <param name="id">ID nhận xét. </param>
    /// <returns>Chi tiết về nhận xét cụ thể. </return>
    [HttpGet("{id:int:min(1)}")]
    [AllowAnonymous]
    public async Task<Result> Get(int id)
    {
        var query = _reviewRepository.Query()
            .Where(c => c.Status == ReviewStatus.Approved && c.Id == id);

        var result = await query
            .Include(c => c.Medias).ThenInclude(c => c.Media)
            .Include(c => c.Replies)
            .Include(c => c.User)
            .Select(c => new ReviewListResult
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedOn = c.CreatedOn,
                Rating = c.Rating,
                Title = c.Title,
                ReviewerName = c.ReviewerName,
                SupportCount = c.SupportCount,
                Avatar = c.User.AvatarUrl,
                ReplieCount = c.Replies.Where(x => x.Status == ReplyStatus.Approved).Count(),
                MediaUrls = c.Medias.OrderBy(x => x.DisplayOrder).Select(x => x.Media.Url)
            }).FirstOrDefaultAsync();
        return Result.Ok(result);
    }
}
