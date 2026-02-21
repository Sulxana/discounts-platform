using MediatR;

namespace Discounts.Application.Settings.Commands.UpdateSetting
{
    public record UpdateSettingCommand(string Key, string Value) : IRequest;
}
