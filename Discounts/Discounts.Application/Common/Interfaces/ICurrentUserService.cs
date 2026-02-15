namespace Discounts.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; } // წამოვა JWT sub claim-დან
        string? Email { get; } 
        bool IsAuthenticated { get; } //valid jwt უნდა ქონდეს
        bool IsInRole(string role);
    }
}
