using Discounts.Application.Common.Interfaces;
using System.Security.Claims;

namespace Discounts.Api.Infrastracture.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private ClaimsPrincipal? User
        {
            get
            {
                return _contextAccessor.HttpContext?.User;
            }
        }

        public Guid? UserId
        {
            get
            {
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");

                return Guid.TryParse(userId, out var parsed) ? parsed : null;
            }
        }

        public string? Email => User?.FindFirstValue(ClaimTypes.Email);

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
    }
}
