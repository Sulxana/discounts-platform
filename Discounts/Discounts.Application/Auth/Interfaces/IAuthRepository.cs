using Discounts.Domain.Auth;

namespace Discounts.Application.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<RefreshToken?> GetRefreshTokenByHashAsync(CancellationToken token, string hash);
        Task AddRefreshTokenAsync(CancellationToken token, RefreshToken refreshToken);
        Task SaveChangesAsync(CancellationToken token);
    }
}
