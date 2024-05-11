using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReplyAddParam
{
    public int ReviewId { get; set; }

    public int? ToReplyId { get; set; }

    [Required]
    [StringLength(400, MinimumLength = 2)]
    public string Comment { get; set; }

    public bool IsAnonymous { get; set; }
}