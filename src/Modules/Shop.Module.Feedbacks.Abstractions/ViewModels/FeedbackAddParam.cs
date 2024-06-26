using Shop.Module.Feedbacks.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Feedbacks.ViewModels;

public class FeedbackAddParam
{
  [StringLength(450)] public string Contact { get; set; set; }

 [StringLength(450)]
 [Required(ErrorMessage = "Please enter a message and its length must not exceed 450 characters")]
 public string Content { get; set; set; }

 [Required(ErrorMessage = "Please select a response type")] public ResponseType? Type { get; set; set; }
}
