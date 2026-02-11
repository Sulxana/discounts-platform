using Discounts.Application.Offers.Interfaces;
using Discounts.Infrastracture.Persistence.Context;
using Discounts.Infrastracture.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastracture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IOfferRepository, OfferRepository>();
            
            services.AddDbContext<DiscountsDbContext>(options => options.UseSqlServer(config.GetConnectionString(nameof(ConnectionStrings.DiscountsDb))));

            return services;
        }
    }
}
