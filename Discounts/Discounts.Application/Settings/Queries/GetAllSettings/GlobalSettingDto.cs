namespace Discounts.Application.Settings.Queries.GetAllSettings
{
    public class GlobalSettingDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
