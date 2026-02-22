using Discounts.Application.MerchantApplications.Interfaces;

namespace Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications
{
    public class GetAllMerchantApplicationsHandler
    {
        private readonly IMerchantApplicationRepository _repository;

        public GetAllMerchantApplicationsHandler(IMerchantApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MerchantApplicationDto>> Handle(GetAllMerchantApplicationsQuery query, CancellationToken cancellationToken)
        {
            var applications = await _repository.GetAllWithUsersAsync(query.Status, query.Page, query.PageSize, cancellationToken).ConfigureAwait(false);

            return applications.Select(x => new MerchantApplicationDto
            {
                Id = x.Application.Id,
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Status = x.Application.Status.ToString(),
                CreatedAt = x.Application.CreatedAtUtc,
                ReviewedAt = x.Application.ReviewedAtUtc,
                RejectionReason = x.Application.RejectionReason
            }).ToList();
        }
    }
}
