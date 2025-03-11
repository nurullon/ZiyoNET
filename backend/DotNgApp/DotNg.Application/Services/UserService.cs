using DotNg.Application.Models;
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
}