using System.ComponentModel;

namespace Shop.Module.Feedbacks.Models;

public enum FeedbackType
{
    [Description("Liên quan đến sản phẩm")] Product = 0,
    [Description("Liên quan đến hậu cần")] Logistics = 1,
    [Description("Dịch vụ khách hàng")] Customer = 2,
    [Description("Khuyến mãi")] Discounts = 3,
    [Description("Chức năng bất thường")] Dysfunction = 4,
    [Description("Đề xuất sản phẩm")] ProductProposal = 5,
    [Description("Khác")] Other = 6
}
