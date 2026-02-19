using Discounts.Application.Common.Exceptions;
using Discounts.Application.Categories.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Domain.Categories;
using MediatR;

namespace Discounts.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _repository;

        public DeleteCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetCategoryByIdAsync(cancellationToken, request.Id);

            if (category == null)
            {
                throw new NotFoundException(nameof(Category), request.Id);
            }

            await _repository.DeleteCategoryAsync(cancellationToken, category);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
