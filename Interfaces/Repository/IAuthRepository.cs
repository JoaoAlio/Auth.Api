using Auth.Api.Model;
using static Auth.Api.Enums;

namespace Auth.Api.Interfaces;

public interface IAuthRepository : IGenericRepository<User>
{
    Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true);
    Task<bool> ExistsUser(string email);
}
