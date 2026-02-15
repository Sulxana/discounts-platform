namespace Discounts.Infrastracture.Auth
{
    public interface IJwtTokenGenerator
    {
        (string Token, string JwtId, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string? email, IReadOnlyCollection<string> roles);

        string GenerateRefreshToken();
    }
}
