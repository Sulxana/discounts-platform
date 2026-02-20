namespace Discounts.Application.Common.Models
{
    public class ErrorResponse
    {
        public string Type { get; set; } = "about:blank";
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string? Instance { get; set; }
        public List<string>? Errors { get; set; }
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
