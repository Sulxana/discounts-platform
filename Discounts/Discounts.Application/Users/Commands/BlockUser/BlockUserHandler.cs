using Discounts.Application.Common.Interfaces;
using MediatR;

namespace Discounts.Application.Users.Commands.BlockUser
{
    public class BlockUserHandler : IRequestHandler<BlockUserCommand>
    {
        private readonly IIdentityService _identityService;

        public BlockUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(BlockUserCommand request, CancellationToken token)
        {
            var result = await _identityService.BlockUserAsync(request.UserId);
            
            if (!result)
            {
                throw new Exception("Failed to block user");
            }
        }
    }
}
