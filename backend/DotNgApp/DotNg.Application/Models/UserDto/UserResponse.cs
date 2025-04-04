using DotNg.Application.Models.RoleDto;

namespace DotNg.Application.Models.UserDto;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? UserName { get; set; } 
    public string? ProfileImageUrl { get; set; }
    public RoleResponse? Role { get; set; } = null;
}