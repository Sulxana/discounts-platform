using MediatR;

namespace Discounts.Application.Users.Commands.BlockUser
{
    public record BlockUserCommand(Guid UserId) : IRequest;
}
