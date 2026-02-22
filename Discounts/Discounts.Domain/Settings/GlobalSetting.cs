namespace Discounts.Domain.Settings
{
    public class GlobalSetting
    {
        private GlobalSetting()
        {

        }
        public GlobalSetting(string key, string value, string description, SettingType type)
        {
            Key = key;
            Value = value;
            Description = description;
            Type = type;
            UpdatedAt = DateTime.UtcNow;
        }

        public string Key { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public SettingType Type { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public void UpdateValue(string newValue)
        {
            Value = newValue;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
