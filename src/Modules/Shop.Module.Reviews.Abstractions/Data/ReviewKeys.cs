using Shop.Module.Core.Data;

namespace Shop.Module.Reviews.Data;

public class ReviewKeys : ShopKeys
{
    public static string Module = System + ":review";

    /// <summary>
    /// Bật kiểm duyệt bình luận tự động
    /// </summary>
    public const string IsReviewAutoApproved = "IsReviewAutoApproved";

    /// <summary>
    /// Bật tự động xem lại câu trả lời
    /// </summary>
    public const string IsReplyAutoApproved = "IsReplyAutoApproved";
}
