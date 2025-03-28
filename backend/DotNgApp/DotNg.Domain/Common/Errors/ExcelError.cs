namespace DotNg.Domain.Common.Errors;

public class ExcelError(string code, string message) : Error(code, message);