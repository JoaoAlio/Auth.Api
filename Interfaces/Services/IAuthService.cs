using Auth.Api.DTOs;
using Auth.Api.Model;
using static Auth.Api.Enums;

namespace Auth.Api.Interfaces;

public interface IAuthService
{
    Task<ResponseModel> Register(RequestRegister user);
    Task<ResponseModel> Login(RequestUserLogin user);
    Task<ResponseModel> ForgotPassword(RequestForgotPassword request);  
    Task<ResponseModel> ResetPassword(RequestResetPassword request);  
    Task<string> Logout();
    Task<User?> GetUserBy(string filter, SearchUserBy searchUserBy, bool asNoTracking = true);
}
