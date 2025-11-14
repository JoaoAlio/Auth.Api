using Auth.Api.Data;
using Auth.Api.Interfaces;
using Auth.Api.Model;
using Microsoft.EntityFrameworkCore;
using static Auth.Api.Enums;

namespace Auth.Api.Repository;

public class AuthRepository : GenericRepository<User>, IAuthRepository
{
    private readonly DataContext _context;
    public AuthRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public Task<bool> ExistsUser(string email)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Email.Equals(email));
    }

    public async Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true)
    {
        if (searchUserBy == SearchUserBy.Id)
        {
            if (!int.TryParse(filter, out int id))
                return null;

            if (asNoTracking)
                return await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

            return await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);
        }

        if (searchUserBy.Equals(SearchUserBy.Name))
        {
            if (asNoTracking)
                return await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Name.Equals(filter));
            else
                return await _context.Users
                .FirstOrDefaultAsync(u => u.Name.Equals(filter));
        }

        if (searchUserBy.Equals(SearchUserBy.Email))
        {
            if(asNoTracking)
                return await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.Equals(filter));
            else
                return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(filter));
        }

        return null;
    }
}
