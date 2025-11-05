using Auth.Api.Model;
using static Auth.Api.Enums;

namespace Auth.Api.Interfaces;

public interface IAuthRepository
{
    Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true);
    Task<bool> ExistsUser(string email);
    Task<bool> SaveAsync(User user);
    Task UpdateAsync(User user);
}
