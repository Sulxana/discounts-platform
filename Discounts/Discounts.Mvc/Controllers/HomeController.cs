using Discounts.Application.Categories.Queries.GetAllCategories;
using Discounts.Application.Offers.Queries.GetActiveOffers;
using Discounts.Domain.Offers;
using Discounts.Mvc.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISender _mediator;

        public HomeController(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(
            string? searchTerm, 
            string? categoryName, 
            decimal? minPrice, 
            decimal? maxPrice, 
            string? sortBy = null,
            int page = 1, 
            CancellationToken token = default)
        {
            var query = new GetActiveOffersQuery(categoryName, minPrice, maxPrice, searchTerm, OfferStatus.Approved, page, 12);
            
            var offers = await _mediator.Send(query, token);
            var categories = await _mediator.Send(new GetAllCategoriesQuery(), token);

            var vm = new HomeViewModel
            {
                Offers = offers,
                Categories = categories,
                Category = categoryName,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                CurrentPage = page
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
