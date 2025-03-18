namespace DotNg.Application.Models.Auth;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public CustomUserResponse User { get; set; } = null!;
}

public class CustomUserResponse
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
}