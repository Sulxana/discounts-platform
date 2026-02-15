namespace Discounts.Domain.Auth;

public class RefreshToken
{
    private RefreshToken()
    {

    }
    public RefreshToken(Guid userId, string tokenHash, string jwtId, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        JwtId = jwtId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public string JwtId { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    
    public void MarkAsUsed()
    {
        UsedAt = DateTime.UtcNow;
    }
    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAt;
    }

    public bool IsActive()
    {
        return RevokedAt == null && UsedAt == null && !IsExpired();
    }

}
