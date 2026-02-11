using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
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
            services.AddScoped<GetOfferByIdHandler>();
            services.AddScoped<UpdateOfferHandler>();
            services.AddScoped<GetAllOffersHandler>();
            return services;
        }
    }
}
