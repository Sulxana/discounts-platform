using Discounts.Application.Offers.Interfaces;
using Discounts.Infrastracture.Repositories;

namespace Discounts.Api.Infrastracture.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IOfferRepository, OfferRepository>();
        }
    }
}
