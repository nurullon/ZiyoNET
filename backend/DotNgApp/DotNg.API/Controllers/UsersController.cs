using DotNg.Application.Models.UserDto;
using DotNg.Application.Serialization;
using DotNg.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNg.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, 
    ResponseSerializer responseSerializer) : Controller
{
    //[HttpGet]
    //public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequest request)
    //{
    //    return responseSerializer.ToActionResult(await userService.GetAllUsersAsync(request));
    //}

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        return responseSerializer.ToActionResult(await userService.GetAllUsersAsync());
    }
}