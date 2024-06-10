using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;

namespace Soul.Shop.Module.Payment.Data
{
    public class PaymentCoDCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                new PaymentProvider("CoD")
                {
                    Name = "Cash On Delivery",
                    LandingViewComponentName = "CoDLanding",
                    ConfigureUrl = "payments-cod-config",
                    IsEnabled = true,
                    AdditionalSettings = null
                }
            );
        }
    }
}
