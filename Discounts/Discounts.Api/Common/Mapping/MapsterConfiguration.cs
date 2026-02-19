using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.Queries;
using Discounts.Domain.Offers;
using Mapster;

namespace Discounts.Application.Common.Mapping
{
    public static class MapsterConfiguration
    {
        public static void RegisterMaps(this IServiceCollection services)
        {
            TypeAdapterConfig<CreateOfferCommand, Offer>
               .NewConfig()
            .ConstructUsing(src => new Offer(
                src.Title,
                src.Description,
                src.CategoryId,
                src.ImageUrl,
                src.OriginalPrice,
                src.DiscountedPrice,
                src.TotalCoupons,
                src.StartDate,
                src.EndDate,
                Guid.Empty // set by handler
            ));

            TypeAdapterConfig<Offer, OfferListItemDto>
                .NewConfig()
                .Map(dest => dest.Category, src => src.Category != null ? src.Category.Name : "Unknown");
        }
    }
}
