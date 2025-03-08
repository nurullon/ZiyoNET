namespace DotNg.Application.Models.Auth;

public class FacebookAuthSettings
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/signin-facebook";
}