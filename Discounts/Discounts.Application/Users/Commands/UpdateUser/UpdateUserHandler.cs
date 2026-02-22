using Discounts.Application.Common.Interfaces;
using MediatR;

namespace Discounts.Application.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken token)
        {
            var result = await _identityService.UpdateUserAsync(request.UserId, request.Email, request.FirstName, request.LastName).ConfigureAwait(false);
            if (!result)
            {
                throw new Exception("Failed to update user");
            }
        }
    }
}
