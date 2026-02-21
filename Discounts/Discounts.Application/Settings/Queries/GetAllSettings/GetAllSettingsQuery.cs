using MediatR;

namespace Discounts.Application.Settings.Queries.GetAllSettings
{
    public record GetAllSettingsQuery() : IRequest<List<GlobalSettingDto>>;

}
