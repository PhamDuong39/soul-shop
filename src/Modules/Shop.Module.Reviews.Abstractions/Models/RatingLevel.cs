using System.ComponentModel;

namespace Shop.Module.Reviews.Models;

public enum RatingLevel
{
    [Description("đánh giá xấu")] Bad = 1,
    [Description("đánh giá trung bình")] Medium = 3,
    [Description("tích cực")] Positive = 5
}
