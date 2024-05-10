using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class PaymentReceivedParam
{
    public int? OrderId { get; set; }

    public string OrderNo { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public decimal? PaymentFeeAmount { get; set; }

    public DateTime? PaymentOn { get; set; }

    public string Note { get; set; }
}