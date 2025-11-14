using Auth.Api.Interfaces;
using Auth.Api.Model;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Api.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private const int TokenByteSize = 64;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
    {
        var rawToken = GenerateRandomToken();
        var tokenHash = ComputeSha256Hex(rawToken);

        var refresh = new RefreshToken
        {
            Token = rawToken, 
            TokenHash = tokenHash,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7) 
        };

        await _refreshTokenRepository.SaveAsync(refresh);
        return refresh;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        var hash = ComputeSha256Hex(token);
        var tokens = await _refreshTokenRepository.GetAllAsync();
        return tokens.FirstOrDefault(t => t.TokenHash == hash);
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken is null || !refreshToken.IsActive)
            return false;

        refreshToken.Revoke();
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        return true;
    }

    public async Task<bool> IsValidAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        return refreshToken != null && refreshToken.IsActive;
    }

    public async Task<bool> RevokeAllForUserAsync(int userId)
    {
        var tokens = await _refreshTokenRepository.GetAllAsync();
        var userTokens = tokens.Where(t => t.UserId == userId && t.IsActive).ToList();
        if (!userTokens.Any())
            return false;

        foreach (var t in userTokens)
        {
            t.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(t);
        }

        return true;
    }

    private static string GenerateRandomToken()
    {
        var bytes = new byte[TokenByteSize];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string ComputeSha256Hex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}