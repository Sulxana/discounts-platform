namespace Discounts.Infrastracture.Auth
{
    public interface IJwtTokenGenerator
    {
        (string Token, string JwtId, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string? email, IList<string> roles);

        string GenerateRefreshToken();
    }
}
    