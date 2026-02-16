using FluentValidation;

namespace Discounts.Application.Reservations.Commands.CreateReservation
{
    public class CreateReservationValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationValidator()
        {
            RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1")
                .LessThanOrEqualTo(10).WithMessage("Cannot reserve more than 10 coupons at once");
        }
    }
}
