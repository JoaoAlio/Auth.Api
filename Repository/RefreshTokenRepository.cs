using Auth.Api.Data;
using Auth.Api.Interfaces;
using Auth.Api.Model;

namespace Auth.Api.Repository;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository 
{
    private readonly DataContext _context;
    public RefreshTokenRepository(DataContext context) : base(context)
    {
        _context = context;
    }
}
