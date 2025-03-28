using DotNg.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace DotNg.Application.Services.Interfaces;

public interface IExcelService
{
    Task<Result<bool>> UploadUsersAsync(IFormFile file);
    Task<Result<string>> ExportUsersToExcelAsync();
}