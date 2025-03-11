using DotNg.Application.Models.UserDto;
using DotNg.Application.Services;
using DotNg.Infrastructure.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace DotNg.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, 
    ResponseSerializer responseSerializer) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequest request)
    {
        return responseSerializer.ToActionResult(await userService.GetAllUsersAsync(request));
    }
}