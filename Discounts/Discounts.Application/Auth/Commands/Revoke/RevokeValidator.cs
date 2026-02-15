using FluentValidation;

namespace Discounts.Application.Auth.Commands.Revoke
{
    public class RevokeValidator : AbstractValidator<RevokeCommand>
    {
        public RevokeValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
