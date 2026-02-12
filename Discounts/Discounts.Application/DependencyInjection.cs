using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries.GetActiveOffers;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetDeletedOffers;
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
            services.AddScoped<GetActiveOffersHandler>();
            services.AddScoped<GetDeletedOffersHandler>();
            services.AddScoped<DeleteOfferHandler>();
            services.AddScoped<ApproveOfferHandler>();
            services.AddScoped<RejectOfferHandler>();
            return services;
        }
    }
}
