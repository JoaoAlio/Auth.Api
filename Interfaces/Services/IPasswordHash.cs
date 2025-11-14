namespace Auth.Api.Interfaces;

public interface IPasswordHash
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string password);
}
