using Discounts.Application.Offers.Commands.CreateOffer;
using Discounts.Application.Offers.DTO.Offer;
using Discounts.Domain.Offers;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                src.Category,
                src.ImageUrl,
                src.OriginalPrice,
                src.DiscountedPrice,
                src.TotalCoupons,
                src.StartDate,
                src.EndDate
            ));

            TypeAdapterConfig<CreateOfferRequestDto, CreateOfferCommand>
            .NewConfig();


        }
    }
}
