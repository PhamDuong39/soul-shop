using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Services;
using Shop.Module.MQ;
using Shop.Module.Reviews.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Events;
using Shop.Module.Reviews.Models;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers;

/// <summary>
/// Bộ điều khiển API trả lời nhận xét, được sử dụng để xử lý các hoạt động trả lời nhận xét.
/// </summary>
[Route("api/replies")]
[Authorize()]
public class ReplyApiController : ControllerBase
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Support> _supportRepository;
    private readonly IRepository<Reply> _replyRepository;
    private readonly IWorkContext _workContext;
    private readonly IMQService _mqService;
    private readonly IAppSettingService _appSettingService;

    public ReplyApiController(
        IRepository<Review> reviewRepository,
        IRepository<Support> supportRepository,
        IRepository<Reply> replyRepository,
        IWorkContext workContext,
        IMQService mqService,
        IAppSettingService appSettingService)
    {
        _reviewRepository = reviewRepository;
        _supportRepository = supportRepository;
        _replyRepository = replyRepository;
        _workContext = workContext;
        _mqService = mqService;
        _appSettingService = appSettingService;
    }


    /// <summary>
    /// Đăng bài trả lời bình luận.
    /// </summary>
    /// <param name="param">Thông số trả lời bình luận. </param>
    /// <returns>Kết quả của thao tác. </return>
    [HttpPost()]
    public async Task<Result> Post([FromBody] ReplyAddParam param)
    {
        var user = await _workContext.GetCurrentOrThrowAsync();
        var reply = new Reply
        {
            ReviewId = param.ReviewId,
            Comment = param.Comment,
            IsAnonymous = param.IsAnonymous,
            ParentId = null,
            UserId = user.Id,
            ReplierName = param.IsAnonymous ? $"{user.FullName.First()}***{user.FullName.Last()}" : user.FullName
        };

        if (param.ToReplyId != null)
        {
            var toReply = await _replyRepository.FirstOrDefaultAsync(param.ToReplyId.Value);
            if (toReply == null) throw new Exception("Tin nhắn trả lời không tồn tại");
            reply.ToUserId = toReply.UserId;
            reply.ToUserName = toReply.ReplierName;
            reply.ParentId = toReply.ParentId ?? toReply.Id;
        }

        _replyRepository.Add(reply);
        await _replyRepository.SaveChangesAsync();

        var isAuto = await _appSettingService.Get<bool>(ReviewKeys.IsReplyAutoApproved);
        if (isAuto)
            await _mqService.Send(QueueKeys.ReplyAutoApproved, new ReplyAutoApprovedEvent()
            {
                ReplyId = reply.Id
            });
        return Result.Ok();
    }

    /// <summary>
    /// Nhận tất cả các câu trả lời đã được phê duyệt của nhận xét được chỉ định trong phân trang.
    /// </summary>
    /// <param name="param">Các tham số phân trang và lọc. </param>
    /// <returns>Chỉ định danh sách trả lời các bình luận. </return>
    [HttpPost("grid")]
    [AllowAnonymous]
    public async Task<Result<StandardTableResult<ReplyListResult>>> Grid(
        [FromBody] StandardTableParam<ReplyQueryParam> param)
    {
        var search = param?.Search;
        if (search == null)
            throw new ArgumentNullException("Ngoại lệ tham số");

        var query = _replyRepository.Query()
            .Where(c => c.Status == ReplyStatus.Approved && c.ParentId == null && c.ReviewId == search.ReviewId);

        var result = await query
            .Include(c => c.User)
            .Include(c => c.Childrens).ThenInclude(c => c.ToUser)
            .ToStandardTableResult(param, c => new ReplyListResult
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedOn = c.CreatedOn,
                SupportCount = c.SupportCount,
                Avatar = c.User.AvatarUrl,
                ReplierName = c.ReplierName,
                Replies = c.Childrens.Where(x => x.Status == ReplyStatus.Approved).OrderByDescending(x => x.Id).Select(
                    x => new ReplyListResult()
                    {
                        Id = x.Id,
                        Comment = x.Comment,
                        ReplierName = x.ReplierName,
                        CreatedOn = x.CreatedOn,
                        SupportCount = x.SupportCount,
                        ToUserName = x.ToUserName
                    })
            });
        return Result.Ok(result);
    }
}
