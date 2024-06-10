using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;

namespace Soul.Shop.Module.Payment.Data
{
    public class PaymentCashfreeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                new PaymentProvider("Cashfree")
                {
                    Name = "Cashfree Payment Gateway",
                    LandingViewComponentName = "CashfreeLanding",
                    ConfigureUrl = "payments-cashfree-config",
                    IsEnabled = true,
                    AdditionalSettings =
                        "{ \"IsSandbox\":true, \"AppId\":\"358035b02486f36ca27904540853\", \"SecretKey\":\"26f48dcd6a27f89f59f28e65849e587916dd57b9\" }"
                }
            );
        }
    }
}
