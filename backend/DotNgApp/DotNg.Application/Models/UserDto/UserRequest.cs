using Microsoft.AspNetCore.Http;

namespace DotNg.Application.Models.UserDto;

public class UserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public IFormFile? ProfileImage { get; set; }
}