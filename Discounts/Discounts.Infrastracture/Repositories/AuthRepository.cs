using Discounts.Application.Auth.Interfaces;
using Discounts.Domain.Auth;
using Discounts.Infrastracture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastracture.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DiscountsDbContext _context;

        public AuthRepository(DiscountsDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(CancellationToken token, RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, token);
        }

        public async Task<RefreshToken?> GetRefreshTokenByHashAsync(CancellationToken token, string hash)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, token);
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
