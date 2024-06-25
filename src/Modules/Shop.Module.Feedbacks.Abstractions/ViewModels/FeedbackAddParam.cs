using Shop.Module.Feedbacks.Models;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Feedbacks.ViewModels;

public class FeedbackAddParam
{
    [StringLength(450)] public string Contact { get; set; }

    [StringLength(450)]
    [Required(ErrorMessage = "Vui lòng nhập nội dung và độ dài nội dung không được vượt quá 450 ký tự")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn loại phản hồi")] public FeedbackType? Type { get; set; }
}
