using Discounts.Application.Auth.DTOs;
using Discounts.Application.Common.Interfaces;

namespace Discounts.Application.Auth.Queries.WhoAmI
{
    public record WhoAmIQuery();

    public class WhoAmIHandler
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public WhoAmIHandler(ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<WhoAmIResponse> Handle(WhoAmIQuery query, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var (isSuccess, _, email, roles) = await _identityService.GetUserByIdAsync(userId.Value);

            if (!isSuccess)
            {
                throw new InvalidOperationException("Failed to retrieve user information.");
            }

            return new WhoAmIResponse
            {
                UserId = userId.Value,
                Email = email,
                Roles = roles
            };
        }
    }
}
