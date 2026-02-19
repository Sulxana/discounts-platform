using MediatR;

namespace Discounts.Application.Users.Commands.UnblockUser
{
    public record UnblockUserCommand(Guid UserId) : IRequest;
}
