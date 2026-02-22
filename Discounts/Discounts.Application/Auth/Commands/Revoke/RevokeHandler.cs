using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Security;
using FluentValidation;

namespace Discounts.Application.Auth.Commands.Revoke
{
    public class RevokeHandler
    {
        private readonly IAuthRepository _authRepository;
        private readonly IValidator<RevokeCommand> _validator;

        public RevokeHandler(IAuthRepository authRepository, IValidator<RevokeCommand> validator)
        {
            _authRepository = authRepository;
            _validator = validator;
        }

        public async Task RevokeToken(RevokeCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token).ConfigureAwait(false);

            var hash = TokenHasher.Sha256Base64(command.RefreshToken);

            var stored = await _authRepository.GetRefreshTokenByHashAsync(token, hash).ConfigureAwait(false);

            if (stored == null) return;
            if (!stored.IsActive()) return;

            stored.Revoke();

            await _authRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}
