namespace DotNg.Domain.Common.Errors;

public class UnauthorizedError() : UserError(ErrorCodes.Unauthorized, "Unauthorized");