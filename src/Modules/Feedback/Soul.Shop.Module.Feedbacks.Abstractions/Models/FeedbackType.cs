using System.ComponentModel;

namespace Soul.Shop.Module.Feedbacks.Abstractions.Models;

public enum FeedbackType
{
    [Description("Product related")] Product = 0,
    [Description("Logistics related")] Logistics = 1,
    [Description("Customer service")] Customer = 2,
    [Description("Promotions")] Discounts = 3,
    [Description("Abnormal function")] Dysfunction = 4,
    [Description("Product suggestions")] ProductProposal = 5,
    [Description("Other")] Other = 6
}