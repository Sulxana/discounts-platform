using Discounts.Application.Categories.Interfaces;
using Discounts.Domain.Categories;
using MediatR;

namespace Discounts.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _repository;

        public CreateCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category(request.Name);

            await _repository.AddCategoryAsync(cancellationToken, category).ConfigureAwait(false);
            await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return category.Id;
        }
    }
}
