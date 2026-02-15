using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Auth.Commands.Register;
using Discounts.Application.Auth.Commands.Login;
using Discounts.Application.Auth.Commands.RefreshTokens;
using Discounts.Application.Auth.Commands.Revoke;
using Discounts.Application.Auth.Queries.WhoAmI;
using Discounts.Application.MerchantApplications.Commands.ApplyMerchant;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
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

            services.AddScoped<RegisterHandler>();
            services.AddScoped<LoginHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<RevokeHandler>();
            services.AddScoped<WhoAmIHandler>();

            services.AddScoped<ApplyMerchantHandler>();
            services.AddScoped<ApproveMerchantApplicationHandler>();
            services.AddScoped<RejectMerchantApplicationHandler>();
            return services;
        }
    }
}
