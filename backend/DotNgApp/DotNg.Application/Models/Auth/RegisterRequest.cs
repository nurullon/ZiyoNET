namespace DotNg.Application.Models.Auth;

public class RegisterRequest
{
    public string UserName { get; set; } = null!;
    public string? Name { get; set; }
    public string Password { get; set; } = null!;
}