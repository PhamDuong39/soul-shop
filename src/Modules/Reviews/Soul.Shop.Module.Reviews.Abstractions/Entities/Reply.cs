using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.Entities;

public class Reply : EntityBase
{
    public Reply()
    {
        Status = ReplyStatus.Pending;
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int? ParentId { get; set; }

    public Reply Parent { get; set; }

    public int ReviewId { get; set; }

    public Review Review { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }

    [StringLength(450)] public string Comment { get; set; }

    [StringLength(450)] public string ReplierName { get; set; }

    [ForeignKey("ToUser")] public int? ToUserId { get; set; }

    [ForeignKey("ToUserId")] public User ToUser { get; set; }

    [StringLength(450)] public string ToUserName { get; set; }

    public ReplyStatus Status { get; set; }

    public bool IsAnonymous { get; set; }

    public int SupportCount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<Reply> Childrens { get; set; } = new List<Reply>();
}