using Discounts.Application.Settings.Interfaces;
using MediatR;

namespace Discounts.Application.Settings.Queries.GetAllSettings
{
    public class GetAllSettingsHandler : IRequestHandler<GetAllSettingsQuery, List<GlobalSettingDto>>
    {
        private readonly IGlobalSettingRepository _repository;
        public GetAllSettingsHandler(IGlobalSettingRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<GlobalSettingDto>> Handle(GetAllSettingsQuery query, CancellationToken token)
        {
            var settings = await _repository.GetAllAsync(token).ConfigureAwait(false);
            return settings.Select(s => new GlobalSettingDto
            {
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                Type = s.Type.ToString(),
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }
    }
}
