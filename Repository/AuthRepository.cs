using Auth.Api.Data;
using Auth.Api.Interfaces;
using Auth.Api.Model;
using Microsoft.EntityFrameworkCore;
using static Auth.Api.Enums;

namespace Auth.Api.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    public AuthRepository(DataContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsUser(string email)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Email.Equals(email));
    }

    public async Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true)
    {
        if (searchUserBy.Equals(SearchUserBy.Id))
        {
            return await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id.Equals(filter));
        }

        if (searchUserBy.Equals(SearchUserBy.Name))
        {
            return await _context.Users.AsNoTracking()
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

    public async Task<bool> SaveAsync(User user)
    {
        await _context.Users.AddAsync(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
