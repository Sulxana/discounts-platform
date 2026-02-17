using Asp.Versioning;
using Discounts.Application.Common.Security;
using Discounts.Application.MerchantApplications.Commands.ApproveMerchantApplication;
using Discounts.Application.MerchantApplications.Commands.RejectMerchantApplication;
using Discounts.Application.MerchantApplications.Queries.GetAllMerchantApplications;
using Discounts.Application.Offers.Commands.ApproveOffer;
using Discounts.Application.Offers.Commands.RejectOffer;
using Discounts.Application.Offers.Queries;
using Discounts.Application.Offers.Queries.GetAllOffers;
using Discounts.Application.Offers.Queries.GetDeletedOffers;
using Discounts.Application.Offers.Queries.GetOfferById;
using Discounts.Domain.MerchantApplications;
using Discounts.Domain.Offers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Discounts.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrator)]
    public class AdminController : ControllerBase
    {
        private readonly GetAllOffersHandler _getAllHandler;
        private readonly GetDeletedOffersHandler _getDeletedHandler;
        private readonly GetOfferByIdHandler _getHandler;
        private readonly ApproveOfferHandler _approveOfferHandler;
        private readonly RejectOfferHandler _rejectOfferHandler;
        private readonly ApproveMerchantApplicationHandler _approveUserHandler;
        private readonly RejectMerchantApplicationHandler _rejectUserHandler;
        private readonly GetAllMerchantApplicationsHandler _getAllMerchantApplicationsHandler;

        public AdminController(GetAllOffersHandler getAllHandler, GetDeletedOffersHandler getDeletedHandler, GetOfferByIdHandler getHandler, ApproveOfferHandler approveOfferHandler, RejectOfferHandler rejectOfferHandler, ApproveMerchantApplicationHandler approveHandler, RejectMerchantApplicationHandler rejectHandler, GetAllMerchantApplicationsHandler getAllMerchantApplicationsHandler)
        {
            _getAllHandler = getAllHandler;
            _getDeletedHandler = getDeletedHandler;
            _getHandler = getHandler;
            _approveOfferHandler = approveOfferHandler;
            _rejectOfferHandler = rejectOfferHandler;
            _approveUserHandler = approveHandler;
            _rejectUserHandler = rejectHandler;
            _getAllMerchantApplicationsHandler = getAllMerchantApplicationsHandler;
        }

        [HttpGet("offers/{id:guid}")]
        public async Task<ActionResult<OfferDetailsDto>> GetOfferIncludingDeletedAsync(CancellationToken token, Guid id)
        {
            var result = await _getHandler.GetOfferIncludingDeletedAsync(token, new GetOfferByIdQuery(id));
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("offers")]
        public async Task<ActionResult<List<OfferListItemDto>>> GetAllOffers(CancellationToken token, [FromQuery] OfferCategory? category,
                                                                            [FromQuery] OfferStatus? status, [FromQuery] bool deleted,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _getAllHandler.GetAllOffers(token, new GetAllOffersQuery(category, status, deleted, page, pageSize));
            return Ok(result);
        }

        [HttpGet("offers/deleted")]
        public async Task<ActionResult<List<OfferListItemDto>>> GetDeletedOffers(CancellationToken token, [FromQuery] OfferCategory? category,
                                                                            [FromQuery] OfferStatus? status,
                                                                            [FromQuery] int page = 1,
                                                                            [FromQuery] int pageSize = 20)
        {
            var result = await _getDeletedHandler.GetDeletedOffers(token, new GetDeletedOffersQuery(category, status, page, pageSize));
            return Ok(result);
        }

        [HttpGet("merchant-applications")]
        public async Task<ActionResult<List<MerchantApplicationDto>>> GetAllMerchantApplications(
            [FromQuery] MerchantApplicationStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken token = default)
        {
            var query = new GetAllMerchantApplicationsQuery(status, page, pageSize);
            var result = await _getAllMerchantApplicationsHandler.Handle(query, token);
            return Ok(result);
        }

        [HttpPut("offers/{id:guid}/approve")]
        public async Task<IActionResult> ApproveOffer(CancellationToken token, Guid id)
        {
            await _approveOfferHandler.ApproveOfferAsync(token, new ApproveOfferCommand(id));
            return NoContent();
        }

        [HttpPut("offers/{id:guid}/reject")]
        public async Task<IActionResult> RejectOffer(CancellationToken token, Guid id, [FromQuery][Required] string reason)
        {
            await _rejectOfferHandler.RejectOfferAsync(token, new RejectOfferCommand(id, reason));
            return NoContent();
        }

        [HttpPut("merchant-applications/{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken token)
        {
            await _approveUserHandler.Handle(new ApproveMerchantApplicationCommand(id), token);
            return NoContent();
        }

        [HttpPut("merchant-applications/{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromQuery][Required] string reason, CancellationToken token)
        {
            await _rejectUserHandler.Handle(new RejectMerchantApplicationCommand(id, reason), token);
            return NoContent();
        }

        
    }
}
