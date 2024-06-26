using Shop.Module.Core.Data;

namespace Shop.Module.Reviews.Data;

public class ReviewKeys : ShopKeys
{
    public static string Module = System + ":review";

    /// <summary>
    /// Enable automatic comment moderation
    /// </summary>
    public const string IsReviewAutoApproved = "IsReviewAutoApproved";

    /// <summary>
    /// Turn on automatic review of answers
    /// </summary>
    public const string IsReplyAutoApproved = "IsReplyAutoApproved";
}
