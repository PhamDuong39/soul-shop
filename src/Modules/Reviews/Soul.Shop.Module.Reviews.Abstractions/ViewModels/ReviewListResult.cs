namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReviewListResult
{
    public int Id { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; }

    public string Comment { get; set; }

    public string ReviewerName { get; set; }

    public string Avatar { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedOnForDate => CreatedOn.ToString("yyyy-MM-dd");

    public int ReplieCount { get; set; }

    public int SupportCount { get; set; }

    public IEnumerable<string> MediaUrls { get; set; } = new List<string>();

    public IEnumerable<ReplyListResult> Replies { get; set; } = new List<ReplyListResult>();
}