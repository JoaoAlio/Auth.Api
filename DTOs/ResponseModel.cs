namespace Auth.Api.DTOs;

public class ResponseModel
{
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    public object Data { get; set; }

    public ResponseModel(string? message = null, int? statusCode = null, object data = default)
    {
        Message = message;
        StatusCode = statusCode;
        Data = data;
    }
}
