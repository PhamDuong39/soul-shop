using System.ComponentModel;

namespace Soul.Shop.Module.Reviews.Abstractions.Models;

public enum RatingLevel
{
    [Description("Bad review")] Bad = 1,
    [Description("Average")] Medium = 3,
    [Description("Good reviews")] Positive = 5
}