using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class EmailSend : EntityBase
{
    public EmailSend()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] public string From { get; set; }


    public string To { get; set; }

    public string Cc { get; set; }

    public string Bcc { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public bool IsHtml { get; set; }

    [StringLength(450)] public string OutId { get; set; }


    [StringLength(450)] public string ReceiptId { get; set; }

    public bool IsSucceed { get; set; }

    public string Message { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}