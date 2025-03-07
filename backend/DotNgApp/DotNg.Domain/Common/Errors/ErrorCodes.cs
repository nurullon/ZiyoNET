namespace DotNg.Domain.Common.Errors;

public static class ErrorCodes
{
    public const string ValidationError = "validation_error";
    public const string PasswordMismatch = "password_mismatch";
    public const string NotFound = "not_found";
    public const string AlreadyExists = "already_exists";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string Conflict = "conflict";
    public const string InternalServerError = "internal_server_error";
    public const string BadRequest = "bad_request";
    public const string Timeout = "timeout";
    public const string TooManyRequests = "too_many_requests";
    public const string ServiceUnavailable = "service_unavailable";
}