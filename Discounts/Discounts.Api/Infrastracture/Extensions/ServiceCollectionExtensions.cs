using Discounts.Api.Infrastracture.Services;
using Discounts.Application.Common.Interfaces;

namespace Discounts.Api.Infrastracture.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
