using MediatR;

namespace Discounts.Application.Users.Commands.UpdateUserRole
{
    public class UpdateUserRoleCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }
}
