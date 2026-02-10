using Discounts.Application.Offers.Commands.CreateOffer;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddScoped<CreateOfferHandler>();
            return services;
        }
    }
}
