namespace Auth.Api.Interfaces;

public interface IEmailValidator
{
    bool IsValidEmail(string email);
}
