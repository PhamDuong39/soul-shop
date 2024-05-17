using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class SmsSend : EntityBase
{
    public SmsSend()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [StringLength(450)] [Required] public string PhoneNumber { set; get; }

    [StringLength(450)] public string Value { get; set; }


    [StringLength(450)] public string SignName { get; set; }


    public SmsTemplateType? TemplateType { get; set; }

    [StringLength(450)] public string TemplateCode { set; get; }


    public string TemplateParam { set; get; }


    [StringLength(450)] public string OutId { get; set; }


    [StringLength(450)] public string ReceiptId { get; set; }

    public bool IsUsed { get; set; }

    public bool IsSucceed { get; set; }

    public bool IsTest { get; set; }

    public string Message { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}