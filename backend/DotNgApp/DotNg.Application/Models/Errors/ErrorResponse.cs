namespace DotNg.Application.Models.Errors;

public class ErrorResponse(string code, string? message)
{
    public string Code { get; set; } = code;

    public string? Message { get; set; } = message;
}