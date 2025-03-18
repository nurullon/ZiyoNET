using DotNg.Application.Models;
using DotNg.Application.Models.RoleDto;
using DotNg.Application.Models.UserDto;
using DotNg.Domain.Common;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNg.Application.Services;

public class UserService(IIdentityService identityService) : IUserService
{
    public async Task<Result<ListResponse<UserResponse>>> GetAllUsersAsync(UserFilterRequest request)
    {
        var pageSize = request.PageSize;
        var pageNumber = request.PageNumber;

        var query = identityService.GetUsers();
        var totalCount = await query.CountAsync();

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query.Pagination(pageNumber, pageSize);
        }

        var users = await query.ToListAsync();
        var response = users.Select(user => new UserResponse
        {
            UserName = user.UserName ?? string.Empty
        }).ToList();

        var listResponse = new ListResponse<UserResponse>(response, totalCount, pageNumber, pageSize);
        return Result<ListResponse<UserResponse>>.Success(listResponse);
    }

    public async Task<Result<ListResponse<UserResponse>>> GetAllUsersAsync()
    {
        var query = identityService.GetUsers();
        var totalCount = await query.CountAsync();

        var users = await query.ToListAsync();

        var response = new List<UserResponse>();
        foreach (var user in users)
        {
            var role = await identityService.GetRoleAsync(user);
            var userResponse = new UserResponse
            {
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Name = user.Name ?? string.Empty,
                Id = user.Id,
                Role = role ?? string.Empty
            };

            response.Add(userResponse);
        }

        var listResponse = new ListResponse<UserResponse>(response, totalCount, null, null);
        return Result<ListResponse<UserResponse>>.Success(listResponse);
    }
}