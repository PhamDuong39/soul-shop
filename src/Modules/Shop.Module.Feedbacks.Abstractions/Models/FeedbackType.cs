using System.ComponentModel;

namespace Shop.Module.Feedbacks.Models;

public enum FeedbackType
{
    [Description("Product Related")] Product = 0,
    [Description("Logistics related")] Logistics = 1,
    [Description("Customer Service")] Customer = 2,
    [Description("Promotion")] Discounts = 3,
    [Description("Abnormal function")] Dysfunction = 4,
    [Description("Product recommendation")] ProductProposal = 5,
    [Description("Other")] Other = 6
}
