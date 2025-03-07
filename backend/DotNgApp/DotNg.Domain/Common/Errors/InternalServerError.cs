namespace DotNg.Domain.Common.Errors;

public class InternalServerError(string message = "Internal Server Error") : ServiceError(ErrorCodes.InternalServerError, message);