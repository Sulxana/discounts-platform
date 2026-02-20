using Asp.Versioning;
using Discounts.Application.Categories.Commands.CreateCategory;
using Discounts.Application.Categories.Commands.DeleteCategory;
using Discounts.Application.Categories.Commands.UpdateCategory;
using Discounts.Application.Categories.Queries.GetAllCategories;
using Discounts.Application.Common.Security;
using Discounts.Application.Offers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
                         
        public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken token)
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery(), token);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryCommand command, CancellationToken token)
        {
            var id = await _mediator.Send(command, token);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken token)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            await _mediator.Send(command, token);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            await _mediator.Send(new DeleteCategoryCommand(id), token);
            return NoContent();
        }
    }
}
