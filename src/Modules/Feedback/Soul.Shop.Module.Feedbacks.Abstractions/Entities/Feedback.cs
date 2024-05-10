using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Feedbacks.Abstractions.Models;

namespace Soul.Shop.Module.Feedbacks.Abstractions.Entities;

public class Feedback : EntityBase
{
    public Feedback()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int? UserId { get; set; }

    public User User { get; set; }

    public string Contact { get; set; }

    [StringLength(450)] public string Title { get; set; }

    [StringLength(450)] public string Content { get; set; }

    public FeedbackType Type { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}