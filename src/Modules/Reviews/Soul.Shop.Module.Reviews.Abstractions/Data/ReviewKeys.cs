using Soul.Shop.Module.Core.Abstractions.Data;

namespace Soul.Shop.Module.Reviews.Abstractions.Data;

public class ReviewKeys : ShopKeys
{
    public static string Module = System + ":review";

    public const string IsReviewAutoApproved = "IsReviewAutoApproved";

    public const string IsReplyAutoApproved = "IsReplyAutoApproved";
}