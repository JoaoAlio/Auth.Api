namespace Auth.Api.DTOs;

public class RequestEmail
{
    public string? To { get; set; }
    public string? From { get; set; }
    public string? Subject { get; set; }
   // public string Body { get; set; }
    public string? ResetLink { get; set; }
}
