using DotNg.Application.Models.Auth;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

//[Route("api/auth")]
//[ApiController]
//public class AuthController(ResponseSerializer responseSerializer, IAuthService authService) : Controller
//{
//    [HttpPost("register")]
//    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
//    {
//        return responseSerializer.Serialize(await authService.RegisterAsync(model));
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginRequest model)
//    {
//        return responseSerializer.Serialize(await authService.LoginAsync(model));
//    }

//    [HttpGet("google-login")]
//    public IActionResult GoogleLogin()
//    {
//        var redirectUri = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
//        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
//        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
//    }

//    [HttpGet("signin-google")]
//    public async Task<IActionResult> GoogleResponse()
//    {
//        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//        if (!authenticateResult.Succeeded) return BadRequest("Google authentication failed.");

//        var user = authenticateResult.Principal;
//        if (user == null || !user.Identity.IsAuthenticated)
//        {
//            return Unauthorized("User is not authenticated.");
//        }

//        var email = user.FindFirst(ClaimTypes.Email)?.Value;
//        var name = user.FindFirst(ClaimTypes.Name)?.Value;

//        var loginRequest = new ExternalLoginRequest { Email = email, Name = name ?? "Unknown" };
//        var token = await authService.ExternalLoginAsync(loginRequest);

//        return Ok(new { email, name });
//    }
//}


[Route("api/auth")]
[ApiController]
public class AuthController(ResponseSerializer responseSerializer, 
    IAuthService authService,
    IGoogleAuthService  googleAuthService) : Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        return responseSerializer.Serialize(await authService.RegisterAsync(model));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        return responseSerializer.Serialize(await authService.LoginAsync(model));
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
        return responseSerializer.Serialize(await googleAuthService.HandleGoogleLoginAsync(HttpContext));
    }
}