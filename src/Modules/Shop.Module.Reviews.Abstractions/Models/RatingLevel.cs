using System.ComponentModel;

namespace Shop.Module.Reviews.Models;

public enum RatingLevel
{
    [Description("bad review")] Bad = 1,
    [Description("average rating")] Medium = 3,
    [Description("positive")] Positive = 5
}
