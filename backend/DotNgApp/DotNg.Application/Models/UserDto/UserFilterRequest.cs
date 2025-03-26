namespace DotNg.Application.Models.UserDto;

public class UserFilterRequest : PaginationRequest
{
    public string? SortColumn { get; set; }
    public bool SortOrder { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
}