namespace DotNg.Domain.Common.Errors;

public class ServiceError(string code, string? message = null) : Error(code, message);