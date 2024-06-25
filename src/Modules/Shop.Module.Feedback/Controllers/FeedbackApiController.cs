using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Feedbacks.Entities;
using Shop.Module.Feedbacks.ViewModels;

namespace Shop.Module.Feedbacks.Controllers;

/// <summary>
/// Bộ điều khiển API phản hồi, được sử dụng để xử lý các yêu cầu liên quan đến phản hồi của người dùng
/// </summary>
[Route("api/feedbacks")]
public class FeedbackApiController : ControllerBase
{
    private readonly IRepository<Feedback> _feedbackRepository;
    private readonly IWorkContext _workContext;

    public FeedbackApiController(
        IRepository<Feedback> feedbackRepository,
        IWorkContext workContext)
    {
        _feedbackRepository = feedbackRepository;
        _workContext = workContext;
    }

    /// <summary>
    /// Nhận và lưu phản hồi của người dùng
    /// </summary>
    /// <param name="param">Thêm tham số cho phản hồi của người dùng, bao gồm thông tin liên hệ, nội dung và loại</param>
    /// <returns>Trả về kết quả phép toán, cho biết thông tin phản hồi đã được lưu thành công hay chưa</returns>
    [HttpPost()]
    public async Task<Result> Post([FromBody] FeedbackAddParam param)
    {
        var user = await _workContext.GetCurrentUserOrNullAsync();
        var model = new Feedback()
        {
            UserId = user?.Id,
            Contact = param.Contact,
            Content = param.Content,
            Type = param.Type.Value
        };
        _feedbackRepository.Add(model);
        await _feedbackRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
