namespace Discounts.Application.Auth.DTOs
{
    public class WhoAmIResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
