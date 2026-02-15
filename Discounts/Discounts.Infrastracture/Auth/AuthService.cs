using Discounts.Application.Auth;
using Discounts.Application.Auth.DTOs;
using Discounts.Application.Common.Security;
using Discounts.Infrastracture.Identity;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Auth
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DiscountsDbContext _context;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly JwtSettings _settings;

        public AuthService(UserManager<ApplicationUser> userManager, DiscountsDbContext context, IJwtTokenGenerator jwtGenerator, JwtSettings settings)
        {
            _userManager = userManager;
            _context = context;
            _jwtGenerator = jwtGenerator;
            _settings = settings;
        }

        public async Task<AuthResponse> RegisterAsync(CancellationToken token, RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentException("Password is required.");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true
            };

            var create = await _userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join(", ", create.Errors.Select(e => e.Description)));

            //Default role - მომხმარებელი
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);

            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            return await IssueTokensAsync(user, token);

        }

        public async Task<AuthResponse> LoginAsync(CancellationToken token, LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(request.Password)) throw new ArgumentException("Password is required.");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidOperationException("Invalid Credentials");

            var ok = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!ok)
                throw new InvalidOperationException("Invalid Credentials");

            return await IssueTokensAsync(user, token);
        }

        public async Task<AuthResponse> RefreshTokenAsync(CancellationToken token, RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ArgumentException("Refresh token is required.");

            var hash = TokenHasher.Sha256Base64(request.RefreshToken);

            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, token);

            if (stored == null)
                throw new InvalidOperationException("Invalid refresh token");
            if (!stored.IsActive())
                throw new InvalidOperationException("Refresh token is not active");

            var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
            if (user == null)
                throw new InvalidOperationException("User Not Found");
            stored.MarkAsUsed();

            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, jwtId, expiresAtUtc) = _jwtGenerator.GenerateAccessToken(user.Id, user.Email, roles);

            var newRefreshRaw = _jwtGenerator.GenerateRefreshToken();
            var newRefreshTokenHash = TokenHasher.Sha256Base64(newRefreshRaw);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

            var newRefresh = new RefreshToken(user.Id, newRefreshTokenHash, jwtId, refreshExpiresAt);

            _context.RefreshTokens.Add(newRefresh);
            await _context.SaveChangesAsync(token);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshRaw,
                ExpiresAt = expiresAtUtc
            };
        }

        public async Task RevokeAsync(CancellationToken token, RevokeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ArgumentException("Refresh token is required.");

            var hash = TokenHasher.Sha256Base64(request.RefreshToken);

            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash, token);

            if (stored == null) return; 
            if (!stored.IsActive()) return;

            stored.Revoke(); 

            await _context.SaveChangesAsync(token);
        }

        private async Task<AuthResponse> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var (accessToken, jwtId, expiresAt) = _jwtGenerator.GenerateAccessToken(user.Id, user.Email, roles);

            var refreshRaw = _jwtGenerator.GenerateRefreshToken();
            var refreshHash = TokenHasher.Sha256Base64(refreshRaw);

            var refreshExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

            var refresh = new RefreshToken(user.Id, refreshHash, jwtId, refreshExpiresAt);

            _context.RefreshTokens.Add(refresh);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshRaw,
                ExpiresAt = expiresAt
            };
        }
    }
}
