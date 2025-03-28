using DotNg.Application.Models;
using DotNg.Application.Models.RoleDto;
using DotNg.Application.Models.UserDto;
using DotNg.Domain.Common;

namespace DotNg.Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(UserRequest request);
    Task<Result<UserResponse>> UpdateUserAsync(string id, UserRequest request);
    Task<Result<UserResponse>> GetUserAsync(string id);
    Task<Result<bool>> DeleteUserAsync(string id);
    Task<Result<ListResponse<UserResponse>>> GetAllUsersAsync(UserFilterRequest request);
    Task<Result<List<RoleResponse>>> GetRolesAsync();
}