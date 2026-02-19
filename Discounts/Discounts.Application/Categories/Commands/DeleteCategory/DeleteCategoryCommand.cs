using MediatR;

namespace Discounts.Application.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest;
}
