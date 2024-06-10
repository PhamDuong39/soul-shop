using System.ComponentModel.DataAnnotations;
using SimplCommerce.Module.Payments.Models;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Orders.Abstractions.Entities;

namespace Soul.Shop.Module.Payment.Abstractions.Entities
{
    public class Payment : EntityBase
    {
        public Payment()
        {
            CreatedOn = DateTimeOffset.Now;
            LatestUpdatedOn = DateTimeOffset.Now;
        }

        public long OrderId { get; set; }

        public Order Order { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LatestUpdatedOn { get; set; }

        public decimal Amount { get; set; }

        public decimal PaymentFee { get; set; }

        [StringLength(450)]
        public string PaymentMethod { get; set; }

        [StringLength(450)]
        public string GatewayTransactionId { get; set; }

        public PaymentStatus Status { get; set; }

        public string FailureMessage { get; set; }
    }
}
