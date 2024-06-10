using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Modules;

namespace Soul.Shop.Module.Payment
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            // serviceCollection.AddTransient<IBraintreeConfiguration, BraintreeConfiguration>();
            //
            // GlobalConfiguration.("simplAdmin.paymentBraintree");
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
