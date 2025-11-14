using Auth.Api.DTOs;
using Auth.Api.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.ApiControllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IGoogleService _googleService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAcessTokenGenerator _accessTokenGenerator;

    public AuthController(IAuthService authService, IGoogleService googleService,
        IRefreshTokenService refreshTokenService, IAcessTokenGenerator accessTokenGenerator)
    {
        _authService = authService;
        _googleService = googleService;
        _refreshTokenService = refreshTokenService;
        _accessTokenGenerator = accessTokenGenerator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RequestRegister request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Register(request);
        if (result.StatusCode != StatusCodes.Status200OK)
        {
            return  BadRequest(result.Message);
        }

        return Ok(result.Message);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestUserLogin user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Login(user);
        if (result.StatusCode != StatusCodes.Status200OK)
        {
            return result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
        }

        var userDb = await _authService.GetUserBy(user.Email!, Auth.Api.Enums.SearchUserBy.Email, false);
        if (userDb == null)
            return BadRequest("User not found after login.");

        var acessToken = _accessTokenGenerator.Generate(userDb.UserIdentifier);
        var refresh = await _refreshTokenService.GenerateRefreshTokenAsync(userDb.Id);

        var response = new ResponseUserLogin
        {
            Name = userDb.Name,
            Tokens = new ResponseTokensJson
            {
                AccessToken = acessToken,
                RefreshToken = refresh.Token
            }
        };

        return Ok(response);
    }

    [HttpGet("google")]
    public async Task<IActionResult> LoginGoogle(string urlRedirect)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (_googleService.IsNotAuthenticated(result))
        {
            return Challenge(GoogleDefaults.AuthenticationScheme);
        }
        else
        {
            var claims = result.Principal.Identities.First().Claims;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            return Redirect(urlRedirect);
        }

    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] RequestForgotPassword request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.ForgotPassword(request);

        if (result.StatusCode != StatusCodes.Status200OK)
        {
            return result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] RequestResetPassword request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.ResetPassword(request);
        if (result.StatusCode != StatusCodes.Status200OK)
        {
            return result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
        }

        return Ok(result.Data);

    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RequestRefreshToken request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("RefreshToken is required.");

        var existing = await _refreshTokenService.GetByTokenAsync(request.RefreshToken);
        if (existing == null || !existing.IsActive)
            return Unauthorized("Invalid refresh token.");

        var user = await _authService.GetUserBy(existing.UserId.ToString(), Auth.Api.Enums.SearchUserBy.Id, false);
        if (user == null)
            return Unauthorized("User not found.");

        await _refreshTokenService.RevokeTokenAsync(request.RefreshToken);
        var newRefresh = await _refreshTokenService.GenerateRefreshTokenAsync(existing.UserId);
        var newAccess = _accessTokenGenerator.Generate(user.UserIdentifier);

        return Ok(new ResponseTokensJson
        {
            AccessToken = newAccess,
            RefreshToken = newRefresh.Token
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RequestRefreshToken request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("RefreshToken is required.");

        var existing = await _refreshTokenService.GetByTokenAsync(request.RefreshToken);
        if (existing == null || !existing.IsActive)
            return Unauthorized("Invalid refresh token.");

        var user = await _authService.GetUserBy(existing.UserId.ToString(), Auth.Api.Enums.SearchUserBy.Id, false);
        if (user == null)
            return Unauthorized("User not found.");

         var revoked = await _refreshTokenService.RevokeAllForUserAsync(user.Id);
        if (!revoked)
            return BadRequest("Invalid or already revoked refresh token.");

        return Ok(new { message = "Logout successfully" });
    }

}
