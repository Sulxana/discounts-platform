namespace Discounts.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool IsSuccess, string? Error, Guid UserId)> CreateUserAsync(string email, string password, string firstName, string lastName, string role);
        Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> LoginUserAsync(string email, string password);
        Task<(bool IsSuccess, Guid UserId, string Email, IList<string> Roles)> GetUserByIdAsync(Guid userId);
        
        Task<bool> UserExistsAsync(string email);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
        Task AddRoleAsync(Guid userId, string role);
        Task RemoveRoleAsync(Guid userId, string role);
        Task<bool> BlockUserAsync(Guid userId);
        Task<bool> UnblockUserAsync(Guid userId);
        Task<bool> UpdateUserAsync(Guid userId, string? email, string? firstName, string? lastName);
        Task<List<(Guid Id, string Email, string FirstName, string LastName, bool IsBlocked, IList<string> Roles)>> GetAllUsersAsync();
    }
}
