using DotNg.Application.Models.Auth;
using DotNg.Application.Serialization;
using DotNg.Application.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNg.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(ResponseSerializer responseSerializer,
    IAuthService authService,
    IFacebookAuthService facebookAuthService,
    IGoogleAuthService googleAuthService) : Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        return responseSerializer.ToActionResult(await authService.RegisterAsync(model));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        return responseSerializer.ToActionResult(await authService.LoginAsync(model));
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {

        var redirectUri = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("signin-google")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return Unauthorized("Facebook authentication failed");

        var user = authenticateResult.Principal;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return Unauthorized("User is not authenticated");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown"),
            new(ClaimTypes.Email, user.FindFirst(ClaimTypes.Email)?.Value ?? ""),
            new(ClaimTypes.NameIdentifier, user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                      new ClaimsPrincipal(claimsIdentity),
                                      authProperties);

        var response = await googleAuthService.HandleGoogleLoginAsync(HttpContext);

        return Content($@"
        <script>
            window.opener.postMessage({{ token: '{response.Value?.Token}' }}, 'https://localhost:4200');
            window.close();
        </script>",
        "text/html");
    }

    [HttpGet("facebook-login")]
    public IActionResult FacebookLogin()
    {
        var redirectUri = Url.Action("FacebookResponse", "Auth", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    [HttpGet("signin-facebook")]
    public async Task<IActionResult> FacebookResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return Unauthorized("Facebook authentication failed");

        var user = authenticateResult.Principal;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return Unauthorized("User is not authenticated");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown"),
            new(ClaimTypes.Email, user.FindFirst(ClaimTypes.Email)?.Value ?? ""),
            new(ClaimTypes.NameIdentifier, user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                      new ClaimsPrincipal(claimsIdentity),
                                      authProperties);

        var response = await facebookAuthService.HandleFacebookLoginAsync(HttpContext);
        return Content($@"
        <script>
            window.opener.postMessage({{ success: true, token: '{response.Value?.Token}' }}, 'https://localhost:4200');
            window.close();
        </script>",
        "text/html");

    }
}