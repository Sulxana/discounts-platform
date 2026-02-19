using Discounts.Application.Categories.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Offers.Queries;
using Mapster;
using MediatR;

namespace Discounts.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;

        public GetAllCategoriesHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllCategoriesAsync(cancellationToken);
            return categories.Adapt<List<CategoryDto>>();
        }
    }
}
