using Discounts.Application.Auth.DTOs;

namespace Discounts.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(CancellationToken token, RegisterRequest request);
        Task<AuthResponse> LoginAsync(CancellationToken token, LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(CancellationToken token, RefreshTokenRequest request);
        Task RevokeAsync(CancellationToken token, RevokeRequest request);
    }
}
