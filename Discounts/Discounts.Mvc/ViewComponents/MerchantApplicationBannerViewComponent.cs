using Discounts.Application.MerchantApplications.Queries.GetUserMerchantApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.Mvc.ViewComponents
{
    public class MerchantApplicationBannerViewComponent : ViewComponent
    {
        private readonly ISender _mediator;

        public MerchantApplicationBannerViewComponent(ISender mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity?.IsAuthenticated == true || User.IsInRole("Merchant") || User.IsInRole("Administrator"))
            {
                return Content(string.Empty);
            }

            var application = await _mediator.Send(new GetUserMerchantApplicationQuery()).ConfigureAwait(false);

            if (application != null && application.Status == "Rejected")
            {
                return View(application);
            }

            return Content(string.Empty);
        }
    }
}
