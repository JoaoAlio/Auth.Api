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
    public readonly IAuthService _authService;
    public readonly IGoogleService _googleService;

    public AuthController(IAuthService authService, IGoogleService googleService)
    {
        _authService = authService;
        _googleService = googleService;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RequestRegister request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Register(request);
        return result.StatusCode == StatusCodes.Status200OK ? Ok(result.Message) : BadRequest(result.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestUserLogin user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Login(user);
        return result.StatusCode == StatusCodes.Status200OK ? Ok(result.Data) 
            : result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
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
        return result.StatusCode == StatusCodes.Status200OK ? Ok(result.Data)
            : result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] RequestResetPassword request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.ResetPassword(request);
        return result.StatusCode == StatusCodes.Status200OK ? Ok(result.Data)
            : result.StatusCode == StatusCodes.Status404NotFound ? NotFound(result.Message) : BadRequest(result.Message);
    }

}
