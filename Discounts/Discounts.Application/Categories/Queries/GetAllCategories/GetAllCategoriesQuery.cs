using Discounts.Application.Offers.Queries;
using MediatR;

namespace Discounts.Application.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
}
