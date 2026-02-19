using MediatR;

namespace Discounts.Application.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string Name) : IRequest;
}
