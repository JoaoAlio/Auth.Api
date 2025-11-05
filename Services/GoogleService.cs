using Auth.Api.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Api.Services;

public class GoogleService : IGoogleService
{
    public bool IsNotAuthenticated(AuthenticateResult result)
    {
        return result == null || !result.Succeeded || result.Principal == null || !result.Principal.Identities.Any(i => i.IsAuthenticated);
    }
}
