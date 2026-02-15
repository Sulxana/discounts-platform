using Discounts.Application.Auth.DTOs;
using Discounts.Application.Auth.Interfaces;
using Discounts.Application.Common.Interfaces;
using Discounts.Application.Common.Security;
using Discounts.Domain.Auth;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Discounts.Application.Auth.Commands.Register
{
    public class RegisterHandler
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IAuthRepository _authRepository;
        private readonly IValidator<RegisterCommand> _validator;
        private readonly JwtSettings _settings;

        public RegisterHandler(IIdentityService identityService, IJwtTokenGenerator jwtGenerator, IAuthRepository authRepository, IValidator<RegisterCommand> validator, IOptions<JwtSettings> settings)
        {
            _identityService = identityService;
            _jwtGenerator = jwtGenerator;
            _authRepository = authRepository;
            _validator = validator;
            _settings = settings.Value;
        }

        public async Task<AuthResponse> Register(RegisterCommand command, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(command, token);

            var (isSuccess, error, userId) = await _identityService.CreateUserAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role);
            
            if (!isSuccess)
            {
                 throw new InvalidOperationException(error);
            }

            var roles = await _identityService.GetUserRolesAsync(userId); 
            
            var (accessToken, jwtId, expiresAt) = _jwtGenerator.GenerateAccessToken(userId, command.Email, roles);

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
