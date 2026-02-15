using Discounts.Application.Auth;
using Discounts.Application.Offers.Interfaces;
using Discounts.Infrastracture.Auth;
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

            services.AddOptions<JwtSettings>().Bind(config.GetSection(JwtSettings.SectionName))
                .Validate(s => !string.IsNullOrWhiteSpace(s.Secret) && s.Secret.Length >= 32 &&
                !string.IsNullOrWhiteSpace(s.Issuer) && !string.IsNullOrWhiteSpace(s.Audience) &&
                s.AccessTokenMinutes > 0 && s.RefreshTokenDays > 0,
                "Invalid Jwt settings. Ensure Secret is 32+ chars, Issuer/Audience set, and durations > 0.")
                .ValidateOnStart();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
