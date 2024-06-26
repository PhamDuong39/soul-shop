using System;
using System.Collections.Generic;

namespace Shop.Module.Reviews.ViewModels;

public class ReplyListResult
{
    public int Id { get; set; }

    public string Comment { get; set; }

    public string Avatar { get; set; }

    public string ReplierName { get; set; }

    public int SupportCount { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedOnForDate => CreatedOn.ToString("yyyy-MM-dd");

    /// <summary>
    /// User to whom the current user replies
    /// </summary>
    public string ToUserName { get; set; }

    public IEnumerable<ReplyListResult> Replies { get; set; } = new List<ReplyListResult>();
}
