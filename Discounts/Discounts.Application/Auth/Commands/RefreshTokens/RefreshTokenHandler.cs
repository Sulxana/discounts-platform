using Discounts.Application.Auth.DTOs;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Domain.Auth;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Discounts.Application.Auth.Commands.RefreshTokens
{
    public class RefreshTokenHandler
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IAuthRepository _authRepository;
        private readonly IValidator<RefreshTokenCommand> _validator;
        private readonly JwtSettings _settings;

        public RefreshTokenHandler(IIdentityService identityService, IJwtTokenGenerator jwtGenerator, IAuthRepository authRepository, IValidator<RefreshTokenCommand> validator, IOptions<JwtSettings> settings)
        {
            _identityService = identityService;
            _jwtGenerator = jwtGenerator;
            _authRepository = authRepository;
            _validator = validator;
            _settings = settings.Value;
        }

        public async Task<AuthResponse> CreateRefreshToken(RefreshTokenCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var hash = TokenHasher.Sha256Base64(command.RefreshToken);

            var stored = await _authRepository.GetRefreshTokenByHashAsync(token, hash);

            if (stored == null)
                throw new InvalidOperationException("Invalid refresh token");

            if (!stored.IsActive())
                throw new InvalidOperationException("Refresh token is not active");

            var (isSuccess, userId, email, roles) = await _identityService.GetUserByIdAsync(stored.UserId);

            if (!isSuccess)
                throw new InvalidOperationException("User Not Found");

            stored.MarkAsUsed();
            
            var (accessToken, jwtId, expiresAtUtc) = _jwtGenerator.GenerateAccessToken(userId, email, roles);

            var newRefreshRaw = _jwtGenerator.GenerateRefreshToken();
            var newRefreshTokenHash = TokenHasher.Sha256Base64(newRefreshRaw);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

            var newRefresh = new Discounts.Domain.Auth.RefreshToken(userId, newRefreshTokenHash, jwtId, refreshExpiresAt);

            await _authRepository.AddRefreshTokenAsync(token, newRefresh);
            await _authRepository.SaveChangesAsync(token);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshRaw,
                ExpiresAt = expiresAtUtc
            };
        }
    }
}
