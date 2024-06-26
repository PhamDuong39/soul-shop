using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Feedbacks.Entities;
using Shop.Module.Feedbacks.ViewModels;

namespace Shop.Module.Feedbacks.Controllers;

/// <summary>
/// Response API controller, used to handle requests related to user responses
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
    /// Receive and save user responses
    /// </summary>
    /// <param name="param">Add parameters for user responses, including contact information, content, and type</param>
    /// <returns>Returns the result of the operation, indicating whether the response information has been saved successfully or not</returns>
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
