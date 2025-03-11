using DotNg.Application.Models;
using DotNg.Application.Models.UserDto;
using DotNg.Domain.Common;

namespace DotNg.Application.Services;

public interface IUserService
{
    Task<Result<ListResponse<UserResponse>>> GetAllUsersAsync(UserFilterRequest request);
}