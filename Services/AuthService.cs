using Auth.Api.DTOs;
using Auth.Api.Interfaces;
using Auth.Api.Model;
using static Auth.Api.Enums;

namespace Auth.Api.Services;

public class AuthService : IAuthService
{
    private readonly IEmailService _emailService;
    private readonly IAcessTokenGenerator _acessTokenGenerator;
    private readonly IAuthRepository _authRepository;
    private readonly IEmailValidator _emailValidator;
    private readonly IPasswordHash _passwordHasher;
    public AuthService(
        IEmailService emailService,
        IAcessTokenGenerator acessTokenGenerator,
        IAuthRepository authRepository,
        IEmailValidator emailValidator,
        IPasswordHash passwordHash)
    {
        _emailService = emailService;
        _acessTokenGenerator = acessTokenGenerator;
        _authRepository = authRepository;
        _emailValidator = emailValidator;
        _passwordHasher = passwordHash;
    }

    public async Task<ResponseModel> ForgotPassword(RequestForgotPassword request)
    {
        var user = await _authRepository.GetUserBy(request.Email!, SearchUserBy.Email);
        if (user is null)
            return new ResponseModel("User not found.", StatusCodes.Status404NotFound);

        var requestEmail = new RequestEmail
        {
            To = user.Email,
            From = "joao.alio13@hotmail.com",
            Subject = "🔒 Redefinição de Senha",
            ResetLink = "test"
        };

        var response = await _emailService.SendEmailAsync(requestEmail);
        return response;
    }

    public async Task<ResponseModel> Login(RequestUserLogin user)
    {
        var userDb = await _authRepository.GetUserBy(user.Email!, SearchUserBy.Email);    
        if (userDb is null)
            return new ResponseModel("User not found.", StatusCodes.Status404NotFound);

        if (userDb.Email != user.Email || !_passwordHasher.VerifyPassword(userDb.Password!, user.Password!))
            return new ResponseModel("invalid username or password.", StatusCodes.Status400BadRequest);

        return new ResponseModel("Login successful.", StatusCodes.Status200OK);
    }

    public async Task<string> Logout()
    {
        return await Task.FromResult<string>("Logout successfully");
    }

    public async Task<ResponseModel> Register(RequestRegister request)
    {
        request.Password = _passwordHasher.HashPassword(request.Password!);
        request.UserIdentifier = Guid.NewGuid();

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            UserIdentifier = request.UserIdentifier
        };

        var result = await _authRepository.SaveAsync(user);
        return  new ResponseModel("Registration completed successfully.", StatusCodes.Status200OK) ;
    }

    public async Task<ResponseModel> ResetPassword(RequestResetPassword request)
    {
        var user = await _authRepository.GetUserBy(request.Email!, SearchUserBy.Email);
        if (user == null)
            return new ResponseModel("User not found.", StatusCodes.Status404NotFound);

        if (request.Password != request.ConfirmPassword)
            return new ResponseModel("Passwords do not match.", StatusCodes.Status400BadRequest);

        user.Password = _passwordHasher.HashPassword(request.Password!);
        await _authRepository.UpdateAsync(user);

        return new ResponseModel("Your password has been changed successfully.", StatusCodes.Status200OK);
    }

    public async Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true)
    {
        return await _authRepository.GetUserBy(filter, searchUserBy, asNoTracking);
    }

}
