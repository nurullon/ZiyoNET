using DotNg.Application.Models.UserDto;
using DotNg.Application.Serialization;
using DotNg.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNg.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, 
    ResponseSerializer responseSerializer) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromForm] UserRequest request)
    {
        return responseSerializer.ToActionResult(await userService.CreateUserAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromForm] UserRequest request)
    {
        return responseSerializer.ToActionResult(await userService.UpdateUserAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        return responseSerializer.ToActionResult(await userService.DeleteUserAsync(id));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        return responseSerializer.ToActionResult(await userService.GetUserAsync(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterRequest request)
    {
        return responseSerializer.ToActionResult(await userService.GetAllUsersAsync(request));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        return responseSerializer.ToActionResult(await userService.GetRolesAsync());
    }
}