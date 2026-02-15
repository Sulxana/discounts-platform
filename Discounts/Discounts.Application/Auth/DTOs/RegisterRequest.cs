namespace Discounts.Application.Auth.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
    }
}
