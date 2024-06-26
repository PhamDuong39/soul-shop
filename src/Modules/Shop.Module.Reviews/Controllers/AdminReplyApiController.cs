﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.MQ;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers;

/// <summary>
/// Admin Reply API Controller, used to handle admin comment replies.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/admin-replies")]
public class AdminReplyApiController : ControllerBase
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Support> _supportRepository;
    private readonly IRepository<Reply> _replyRepository;
    private readonly IWorkContext _workContext;
    private readonly IMQService _mqService;

    public AdminReplyApiController(
        IRepository<Review> reviewRepository,
        IRepository<Support> supportRepository,
        IRepository<Reply> replyRepository,
        IWorkContext workContext,
        IMQService mqService)
    {
        _reviewRepository = reviewRepository;
        _supportRepository = supportRepository;
        _replyRepository = replyRepository;
        _workContext = workContext;
        _mqService = mqService;
    }

    /// <summary>
    /// Get the list of admin responses in pagination.
    /// </summary>
    /// <param name="param">Paging and filtering parameters. </param>
    /// <returns>A paginated list of admin responses. </return>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<AdminReplyListResult>>> Grid(
        [FromBody] StandardTableParam<AdminReplyQueryParam> param)
    {
        var query = _replyRepository.Query();
        var search = param?.Search;
        if (search != null)
        {
            if (search.Status.HasValue) query = query.Where(c => c.Status == search.Status.Value);

            if (!string.IsNullOrWhiteSpace(search.ReplierName))
                query = query.Where(c => c.ReplierName.Contains(search.ReplierName));
        }

        var result = await query
            .ToStandardTableResult(param, c => new AdminReplyListResult
            {
                Id = c.Id,
                Comment = c.Comment,
                CreatedOn = c.CreatedOn,
                SupportCount = c.SupportCount,
                ReplierName = c.ReplierName,
                IsAnonymous = c.IsAnonymous,
                ParentId = c.ParentId,
                ReviewId = c.ReviewId,
                Status = c.Status,
                ToUserId = c.ToUserId,
                ToUserName = c.ToUserName,
                UpdatedOn = c.UpdatedOn,
                UserId = c.UserId
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Updates the admin response status with the specified ID.
    /// </summary>
    /// <param name="id">Administrator response ID. </param>
    /// <param name="param">Reply to update parameters. </param>
    /// <returns>The result of the update operation. </return>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] AdminReplyUpdateParam param)
    {
        var user = await _workContext.GetCurrentOrThrowAsync();
        var model = await _replyRepository.FirstOrDefaultAsync(id);
        if (model != null)
        {
            model.Status = param.Status;
            model.UpdatedOn = DateTime.Now;
            await _replyRepository.SaveChangesAsync();
        }

        return Result.Ok();
    }

    /// <summary>
    /// Delete the admin response with the specified ID.
    /// </summary>
    /// <param name="id">Administrator response ID. </param>
    /// <returns>Result of the delete operation. </return>
    [HttpDelete("{id}")]
    public async Task<Result> Delete(int id)
    {
        var user = await _workContext.GetCurrentOrThrowAsync();
        var model = await _replyRepository.FirstOrDefaultAsync(id);
        if (model != null)
        {
            var any = _replyRepository.Query().Any(c => c.ParentId == model.Id);
            if (any) throw new Exception("There is a sub answer below the current answer and deletion is not allowed.");

            model.IsDeleted = true;
            model.UpdatedOn = DateTime.Now;
            await _replyRepository.SaveChangesAsync();
        }

        return Result.Ok();
    }
}
