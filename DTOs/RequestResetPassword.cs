namespace Auth.Api.DTOs;

public class RequestResetPassword
{
    public string? Email { get; set; }
    public string? Password { get; set; }                     
    public string? ConfirmPassword { get; set; }                     
}
