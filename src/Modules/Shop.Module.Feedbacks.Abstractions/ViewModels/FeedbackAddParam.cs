using System.ComponentModel.DataAnnotations;
using Shop.Module.Feedbacks.Models;

namespace Shop.Module.Feedbacks.Abstractions.ViewModels;

public class FeedbackAddParam
{
    [StringLength(450)] public string Contact { get; set; }

    [StringLength(450)]
    [Required(ErrorMessage = "Please enter feedback content")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Please select feedback type")]
    public FeedbackType? Type { get; set; }
}
