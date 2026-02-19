using MediatR;

namespace Discounts.Application.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name) : IRequest<Guid>;
}
