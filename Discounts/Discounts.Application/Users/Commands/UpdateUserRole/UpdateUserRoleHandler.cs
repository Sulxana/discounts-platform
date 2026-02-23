using Discounts.Application.Common.Interfaces;
using MediatR;

namespace Discounts.Application.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleHandler : IRequestHandler<UpdateUserRoleCommand>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserRoleHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            // Fetch all current roles for this user
            var currentRoles = await _identityService.GetUserRolesAsync(request.UserId).ConfigureAwait(false);

            // Remove every existing role
            foreach (var role in currentRoles)
            {
                await _identityService.RemoveRoleAsync(request.UserId, role).ConfigureAwait(false);
            }

            // Assign the single new role
            await _identityService.AddRoleAsync(request.UserId, request.NewRole).ConfigureAwait(false);
        }
    }
}
