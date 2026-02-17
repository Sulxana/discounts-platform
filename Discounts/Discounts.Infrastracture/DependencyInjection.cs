using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Interfaces;
using Discounts.Application.Offers.Interfaces;
using Discounts.Application.Reservations.Interfaces;
using Discounts.Application.Settings.Interfaces;
using Discounts.Infrastracture.Auth;
using Discounts.Infrastracture.Identity;
using Discounts.Infrastracture.Persistence.Context;
using Discounts.Infrastracture.Persistence.Repositories;
using Discounts.Infrastracture.Repositories;
using Discounts.Infrastracture.Services;
using Microsoft.AspNetCore.Identity;
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
            services.AddScoped<IMerchantApplicationRepository, MerchantApplicationRepository>();

            services.AddDbContext<DiscountsDbContext>(options => options.UseSqlServer(config.GetConnectionString(nameof(ConnectionStrings.DiscountsDb))));

            services.AddOptions<JwtSettings>().Bind(config.GetSection(JwtSettings.SectionName))
                .Validate(s => !string.IsNullOrWhiteSpace(s.Secret) && s.Secret.Length >= 32 &&
                !string.IsNullOrWhiteSpace(s.Issuer) && !string.IsNullOrWhiteSpace(s.Audience) &&
                s.AccessTokenMinutes > 0 && s.RefreshTokenDays > 0,
                "Invalid Jwt settings. Ensure Secret is 32+ chars, Issuer/Audience set, and durations > 0.")
                .ValidateOnStart();

            // Identity (USERS And ROLES)
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DiscountsDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();

            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddScoped<IGlobalSettingsService, GlobalSettingsService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMemoryCache();


            return services;
        }
    }
}
