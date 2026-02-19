using Discounts.Application.Common.Exceptions;
using Discounts.Application.Categories.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Domain.Categories;
using MediatR;

namespace Discounts.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _repository;

        public UpdateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetCategoryByIdAsync(cancellationToken, request.Id);

            if (category == null)
            {
                throw new NotFoundException(nameof(Category), request.Id);
            }

            category.Name = request.Name;

            await _repository.UpdateCategoryAsync(cancellationToken, category);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
