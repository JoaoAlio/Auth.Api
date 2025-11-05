using Auth.Api.Interfaces;
using System.Text.RegularExpressions;

namespace Auth.Api.Services;

public class EmailValidator : IEmailValidator
{
    public bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }
}
