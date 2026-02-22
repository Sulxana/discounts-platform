using Discounts.Application.Categories.Commands.CreateCategory;
using Discounts.Application.Categories.Commands.DeleteCategory;
using Discounts.Application.Categories.Commands.UpdateCategory;
using Discounts.Application.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminCategoriesController : Controller
    {
        private readonly ISender _mediator;

        public AdminCategoriesController(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(), token).ConfigureAwait(false);
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Category name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediator.Send(new CreateCategoryCommand(name), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Category created successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string name, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Category name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _mediator.Send(new UpdateCategoryCommand(id, name), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Category updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            try
            {
                await _mediator.Send(new DeleteCategoryCommand(id), token).ConfigureAwait(false);
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
