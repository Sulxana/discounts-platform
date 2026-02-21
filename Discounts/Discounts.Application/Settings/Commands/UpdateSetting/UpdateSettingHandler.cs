using Discounts.Application.Settings.Interfaces;
using MediatR;

namespace Discounts.Application.Settings.Commands.UpdateSetting
{
    public class UpdateSettingHandler : IRequestHandler<UpdateSettingCommand>
    {
        private readonly IGlobalSettingRepository _repository;
        private readonly IGlobalSettingsService _settingsService;

        public UpdateSettingHandler(IGlobalSettingRepository repository, IGlobalSettingsService settingsService)
        {
            _repository = repository;
            _settingsService = settingsService;
        }

        public async Task Handle(UpdateSettingCommand command, CancellationToken token)
        {
            var normalizedKey = command.Key?.Trim().ToLowerInvariant() ?? string.Empty;
            var trimmedValue = command.Value?.Trim() ?? string.Empty;

            var setting = await _repository.GetByKeyAsync(token, command.Key);

            if (setting == null)
                throw new InvalidOperationException($"Setting with key '{command.Key}' not found");
            
            setting.UpdateValue(trimmedValue);

            await _repository.SaveChangesAsync(token);
            _settingsService.RemoveFromCache(normalizedKey);
        }
    }
}
