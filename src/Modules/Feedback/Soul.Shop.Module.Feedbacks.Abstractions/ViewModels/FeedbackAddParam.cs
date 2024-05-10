using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Feedbacks.Abstractions.Models;

namespace Soul.Shop.Module.Feedbacks.Abstractions.ViewModels;

public class FeedbackAddParam
{
    [StringLength(450)] public string Contact { get; set; }

    [StringLength(450)]
    [Required(ErrorMessage = "Please enter content, and the content length cannot exceed 450 characters")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Please select feedback type")]
    public FeedbackType? Type { get; set; }
}