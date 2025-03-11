namespace DotNg.Application.Models.UserDto;

public class UserFilterRequest : PaginationRequest
{
    public string? UserName { get; set; }
}