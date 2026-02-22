using Discounts.Application.Common.Interfaces;
using Discounts.Application.MerchantApplications.Interfaces;
using MediatR;

namespace Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication
{
    public class GetUserMerchantApplicationHandler : IRequestHandler<GetUserMerchantApplicationQuery, UserMerchantApplicationDto?>
    {
        private readonly IMerchantApplicationRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserMerchantApplicationHandler(IMerchantApplicationRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<UserMerchantApplicationDto?> Handle(GetUserMerchantApplicationQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) return null;

            var application = await _repository.GetByUserIdAsync(userId.Value, cancellationToken).ConfigureAwait(false);

            if (application == null) return null;

            return new UserMerchantApplicationDto
            {
                Id = application.Id,
                Status = application.Status.ToString(),
                RejectionReason = application.RejectionReason,
                CreatedAt = application.CreatedAtUtc
            };
        }
    }
}
