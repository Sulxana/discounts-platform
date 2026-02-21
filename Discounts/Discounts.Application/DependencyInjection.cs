using Discounts.Application.Auth.Commands.Login;
using Discounts.Application.Auth.Commands.RefreshTokens;
using Discounts.Application.Auth.Commands.Register;
using Discounts.Application.Auth.Commands.Revoke;
using Discounts.Application.Auth.Queries.WhoAmI;
using Discounts.Application.MerchantApplications.Commands.ApplyMerchant;
using Discounts.Application.Coupons.Commands.RedeemCoupon;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;
using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Commands.DeleteOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Commands.UpdateOffer;
using Discounts.Application.Offers.Queries.GetActiveOffers;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetDeletedOffers;
using Discounts.Application.Offers.Queries.GetMerchantDashboardStats;
using Discounts.Application.Offers.Queries.GetMerchantOffers;
using Discounts.Application.Offers.Queries.GetMerchantSalesHistory;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Application.Reservations.Commands.CancelReservation;
using Discounts.Application.Reservations.Commands.CreateReservation;
using Discounts.Application.Reservations.Queries.GetUserReservations;
using Discounts.Application.Settings.Commands.UpdateSetting;
using Discounts.Application.Settings.Queries.GetAllSettings;
using Discounts.Application.Reservations.Services;
using Discounts.Application.Offers.Services;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Application.Reservations.Commands.PurchaseReservation;
using Discounts.Application.Coupons.Commands.DirectPurchase;
using Discounts.Application.Coupons.Queries.GetMyCoupons;
using Discounts.Application.Coupons.Queries.GetUserCoupons;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Discounts.Application.Users.Commands.BlockUser;
using Discounts.Application.Users.Commands.UpdateUser;
using Discounts.Application.Users.Commands.UnblockUser;
using Discounts.Application.Categories.Commands.CreateCategory;
using Discounts.Application.Categories.Commands.UpdateCategory;
using Discounts.Application.Categories.Commands.DeleteCategory;
using Discounts.Application.Categories.Queries.GetAllCategories;

namespace Discounts.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddScoped<CreateOfferHandler>();
            services.AddScoped<GetOfferByIdHandler>();
            services.AddScoped<UpdateOfferHandler>();
            services.AddScoped<GetAllOffersHandler>();
            services.AddScoped<GetActiveOffersHandler>();
            services.AddScoped<GetDeletedOffersHandler>();
            services.AddScoped<GetMerchantOffersHandler>();
            services.AddScoped<DeleteOfferHandler>();
            services.AddScoped<ApproveOfferHandler>();
            services.AddScoped<RejectOfferHandler>();
            services.AddScoped<GetMerchantDashboardStatsHandler>();
            services.AddScoped<GetMerchantSalesHistoryHandler>();

            services.AddScoped<RegisterHandler>();
            services.AddScoped<LoginHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<RevokeHandler>();
            services.AddScoped<WhoAmIHandler>();

            services.AddScoped<ApplyMerchantHandler>();
            services.AddScoped<ApproveMerchantApplicationHandler>();
            services.AddScoped<RejectMerchantApplicationHandler>();
            services.AddScoped<GetAllMerchantApplicationsHandler>();
            services.AddScoped<Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication.GetUserMerchantApplicationHandler>();

            services.AddScoped<CreateReservationHandler>();
            services.AddScoped<CancelReservationHandler>();
            services.AddScoped<GetUserReservationsHandler>();

            services.AddScoped<GetAllSettingsHandler>();
            services.AddScoped<UpdateSettingHandler>();

            services.AddScoped<IReservationCleanupService, ReservationCleanupService>();
            services.AddScoped<IOfferCleanupService, OfferCleanupService>();
            
            services.AddScoped<PurchaseReservationHandler>();
            services.AddScoped<DirectPurchaseHandler>();
            services.AddScoped<GetMyCouponsHandler>();
            services.AddScoped<GetUserCouponsHandler>();
            services.AddScoped<RedeemCouponHandler>();

            services.AddScoped<BlockUserHandler>();
            services.AddScoped<UnblockUserHandler>();
            services.AddScoped<UpdateUserHandler>();

            services.AddScoped<CreateCategoryHandler>();
            services.AddScoped<UpdateCategoryHandler>();
            services.AddScoped<DeleteCategoryHandler>();
            services.AddScoped<GetAllCategoriesHandler>();

            return services;
        }
    }
}
