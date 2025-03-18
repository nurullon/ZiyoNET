namespace DotNg.Domain.Common.Errors;

public class ValidationError(List<string> errors, string message = "Validation failed") : UserError(ErrorCodes.BadRequest, message)
{
    public List<string> Errors { get; set; } = errors;
}
