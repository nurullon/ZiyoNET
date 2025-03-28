using AutoMapper;
using DotNg.Application.Models;
using DotNg.Application.Models.RoleDto;
using DotNg.Application.Models.UserDto;
using DotNg.Application.Services.Interfaces;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using DotNg.Domain.Common.Messages;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace DotNg.Application.Services;

public class UserService(IIdentityService identityService,
    IMapper mapper,
    IPasswordHasher passwordHasher) : IUserService
{
    public async Task<Result<UserResponse>> CreateUserAsync(UserRequest request)
    {
        var existingUser = await identityService.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.AlreadyExists, 
                UserErrorMessages.UserAlreadyExists));

        var user = new AppUser
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.UserName
        };

        var result = await identityService.CreateUserAsync(user, request.Password);
        if (!result.Succeeded)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.ValidationError, 
                result.Errors.First().Description));

        var role = await identityService.GetRoles().FirstOrDefaultAsync(r => r.Id == request.RoleId);
        if (role == null)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.NotFound,
                UserErrorMessages.RoleNotFound));

        await identityService.AddToRoleAsync(user, role.Name);

        return Result<UserResponse>.Success(new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserName = user.UserName,
            Role = mapper.Map<RoleResponse>(role)
        });
    }

    public async Task<Result<UserResponse>> GetUserAsync(string id)
    {
        var user = await identityService.GetUsers().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.NotFound, 
                UserErrorMessages.UserNotFound));

        var role = await identityService.GetRoleAsync(user);

        return Result<UserResponse>.Success(new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserName = user.UserName,
            Role = mapper.Map<RoleResponse>(role)
        });
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(string id, UserRequest request)
    {
        var user = await identityService.GetUsers().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.NotFound, 
                UserErrorMessages.UserNotFound));

        user.Name = request.Name;
        user.Email = request.Email;
        user.UserName = request.UserName;

        await identityService.UpdateUserAsync(user);

        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = passwordHasher.HashPassword(request.Password);

        var role = await identityService.GetRoles().FirstOrDefaultAsync(r => r.Id == request.RoleId);
        if (role == null)
            return Result.Fail<UserResponse>(new UserError(
                ErrorCodes.NotFound,
                UserErrorMessages.RoleNotFound));

        await identityService.UpdateUserRoleAsync(user, role.Name);

        return Result<UserResponse>.Success(new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            UserName = user.UserName,
            Role = mapper.Map<RoleResponse>(role)
        });
    }

    public async Task<Result<ListResponse<UserResponse>>> GetAllUsersAsync(UserFilterRequest request)
    {
        var query = identityService.GetUsers();

        if (!string.IsNullOrEmpty(request.Name))
            query = query.Where(u => u.Name.ToLower().StartsWith(request.Name.ToLower()));

        if (!string.IsNullOrEmpty(request.Email))
            query = query.Where(u => u.Email != null && u.Email.ToLower().StartsWith(request.Email.ToLower()));

        if (!string.IsNullOrEmpty(request.UserName))
            query = query.Where(u => u.UserName != null && u.UserName.ToLower().StartsWith(request.UserName.ToLower()));

        if (!string.IsNullOrEmpty(request.SortColumn))
            query = request.SortOrder
                ? query.OrderByDynamic(request.SortColumn, true)
                : query.OrderByDynamic(request.SortColumn, false);

        var totalCount = await query.CountAsync();
        var users = await query.Pagination(request.PageNumber, request.PageSize).ToListAsync();

        var response = new List<UserResponse>();

        foreach (var user in users)
        {
            var role = await identityService.GetRoleAsync(user);

            response.Add(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Role = mapper.Map<RoleResponse>(role)
            });
        }

        var result = new ListResponse<UserResponse>(response, totalCount, request.PageNumber, request.PageSize);

        return Result<ListResponse<UserResponse>>.Success(result);
    }

    public async Task<Result<List<RoleResponse>>> GetRolesAsync()
    {
        var roles = await identityService.GetRoles().ToListAsync();
        var response = mapper.Map<List<RoleResponse>>(roles);
        return Result<List<RoleResponse>>.Success(response);
    }

    public async Task<Result<bool>> DeleteUserAsync(string id)
    {
        var user = identityService.GetUsers().FirstOrDefault(u => u.Id == id);
        if (user == null)
            return Result.Fail<bool>(new UserError(
                ErrorCodes.NotFound,
                UserErrorMessages.UserNotFound));

        var result = await identityService.DeleteUserAsync(user);
        return Result<bool>.Success(result);
    }
}