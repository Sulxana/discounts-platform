using Discounts.Application.Auth.DTOs;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using FluentValidation;
using Microsoft.Extensions.Options;
using Discounts.Domain.Auth;

namespace Discounts.Application.Auth.Commands.Login
{
    public class LoginHandler
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IAuthRepository _authRepository;
        private readonly IValidator<LoginCommand> _validator;
        private readonly JwtSettings _settings;

        public LoginHandler(IIdentityService identityService, IJwtTokenGenerator jwtGenerator, IAuthRepository authRepository, IValidator<LoginCommand> validator, IOptions<JwtSettings> settings)
        {
            _identityService = identityService;
            _jwtGenerator = jwtGenerator;
            _authRepository = authRepository;
            _validator = validator;
            _settings = settings.Value;
        }

        public async Task<AuthResponse> Login(LoginCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var (isSuccess, userId, email, roles) = await _identityService.LoginUserAsync(command.Email, command.Password);

            if (!isSuccess)
            {
                throw new InvalidOperationException("Invalid Credentials");
            }

            var (accessToken, jwtId, expiresAt) = _jwtGenerator.GenerateAccessToken(userId, email, roles);

            var refreshRaw = _jwtGenerator.GenerateRefreshToken();
            var refreshHash = TokenHasher.Sha256Base64(refreshRaw);

            var refreshExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

            var refresh = new RefreshToken(userId, refreshHash, jwtId, refreshExpiresAt);

            await _authRepository.AddRefreshTokenAsync(token, refresh);
            await _authRepository.SaveChangesAsync(token);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshRaw,
                ExpiresAt = expiresAt
            };
        }
    }
}
