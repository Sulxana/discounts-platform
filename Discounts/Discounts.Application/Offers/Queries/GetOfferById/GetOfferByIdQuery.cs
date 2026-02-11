using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discounts.Application.Offers.Queries.GetOfferById
{
    public class GetOfferByIdQuery
    {
        public Guid Id { get;}
        public GetOfferByIdQuery(Guid id)
        {
            Id = id;
        }
    }

}
