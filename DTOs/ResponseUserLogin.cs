namespace Auth.Api.DTOs;

public class ResponseUserLogin
{
    public string? Name  { get; set; }
    public ResponseTokensJson? Tokens  { get; set; }
}
