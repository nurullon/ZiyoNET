namespace DotNg.Application.Models.Auth;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}