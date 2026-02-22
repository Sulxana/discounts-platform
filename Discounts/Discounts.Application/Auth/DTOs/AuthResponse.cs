namespace Discounts.Application.Auth.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; init; } = null!;
        public string RefreshToken { get; init; } = null!;
        public DateTime ExpiresAt { get; init; }
    }
}
