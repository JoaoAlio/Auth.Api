using Microsoft.AspNetCore.Authentication;

namespace Auth.Api.Interfaces;

public interface IGoogleService
{
    bool IsNotAuthenticated(AuthenticateResult result);
}
