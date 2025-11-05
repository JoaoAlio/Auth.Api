using Auth.Api.DTOs;

namespace Auth.Api.Interfaces;

public interface IEmailService
{
    Task<ResponseModel> SendEmailAsync(RequestEmail request);
}
