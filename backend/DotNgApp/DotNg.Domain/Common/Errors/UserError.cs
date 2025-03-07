namespace DotNg.Domain.Common.Errors;

public class UserError(string code, string message) : Error(code, message);