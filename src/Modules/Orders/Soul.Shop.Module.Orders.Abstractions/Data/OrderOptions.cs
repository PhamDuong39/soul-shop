namespace Soul.Shop.Module.Orders.Abstractions.Data;

public class OrderOptions
{
    public int OrderAutoCanceledTimeForMinute { get; set; } = 120;

    public int OrderAutoCompleteTimeForMinute { get; set; } = 10080;

    public int OrderCompleteAutoReviewTimeForMinute { get; set; } = 10080;
}