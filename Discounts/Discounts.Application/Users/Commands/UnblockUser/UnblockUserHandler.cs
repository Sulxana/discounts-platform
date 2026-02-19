using Discounts.Application.Common.Interfaces;
using MediatR;

namespace Discounts.Application.Users.Commands.UnblockUser
{
    public class UnblockUserHandler : IRequestHandler<UnblockUserCommand>
    {
        private readonly IIdentityService _identityService;

        public UnblockUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.UnblockUserAsync(request.UserId);

            if (!result)
            {
                throw new Exception("Failed to unblock user");
            }
        }
    }
}
