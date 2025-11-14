using Auth.Api.Model;
using static Auth.Api.Enums;

namespace Auth.Api.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
    Task<bool> IsValidAsync(string token);
    Task<bool> RevokeAllForUserAsync(int userId);
}
