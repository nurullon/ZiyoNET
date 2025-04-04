using DotNg.Application.Services.Interfaces;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using DotNg.Domain.Common.Messages;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace DotNg.Application.Services;

public class ExcelService(IIdentityService identityService) : IExcelService
{
    public async Task<Result<string>> ExportUsersToExcelAsync()
    {
        var users = await identityService.GetUsers().ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Users");

        var headers = new[] { "ID", "Name", "Email", "Username", "Role" };

        var headerColor = ColorTranslator.FromHtml("#2F5597");  
        var textColor = ColorTranslator.FromHtml("#4A4A4A");  

        for (int col = 1; col <= headers.Length; col++)
        {
            worksheet.Cells[1, col].Value = headers[col - 1];
            worksheet.Cells[1, col].Style.Font.Bold = true;
            worksheet.Cells[1, col].Style.Font.Color.SetColor(headerColor); 
            worksheet.Cells[1, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, col].AutoFitColumns();
        }

        int row = 2;
        foreach (var user in users)
        {
            var role = await identityService.GetRoleAsync(user);

            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = user.Name;
            worksheet.Cells[row, 3].Value = user.Email;
            worksheet.Cells[row, 4].Value = user.UserName;
            worksheet.Cells[row, 5].Value = role?.Name;

            for (int col = 1; col <= 5; col++)
            {
                worksheet.Cells[row, col].Style.Font.Color.SetColor(textColor);
                worksheet.Cells[row, col].AutoFitColumns();
            }

            row++;
        }

        string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Users");
        if (!Directory.Exists(wwwRootPath))
            Directory.CreateDirectory(wwwRootPath);

        string fileName = $"Users_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        string filePath = Path.Combine(wwwRootPath, fileName);

        await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());

        string fileUrl = $"/Users/{fileName}";
        return Result<string>.Success(fileUrl);
    }
    
    public async Task<Result<bool>> UploadUsersAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Result.Fail<bool>(new ExcelError(
                ErrorCodes.InvalidFile,
                ExcelErrorMessages.InvalidFile));

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var package = new ExcelPackage(stream);

        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        if (worksheet == null)
        {
            return Result.Fail<bool>(new ExcelError(
                ErrorCodes.InvalidFile,
                ExcelErrorMessages.WorksheetNotFound));
        }

        int rowCount = worksheet.Dimension.Rows;

        var roles = await identityService.GetRoles().ToListAsync();

        for (int row = 2; row <= rowCount; row++)
        {
            var roleName = worksheet.Cells[row, 5].Text;
            var role = roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                return Result.Fail<bool>(new ExcelError(
                    ErrorCodes.NotFound,
                    UserErrorMessages.RoleNotFound));
            }

            var user = new AppUser
            {
                Name = worksheet.Cells[row, 2].Text,
                Email = worksheet.Cells[row, 3].Text,
                UserName = worksheet.Cells[row, 4].Text
            };

            var password = worksheet.Cells[row, 6].Text;

            var result = await identityService.CreateUserAsync(user, password);
            if (!result.Succeeded)
            {
                Console.WriteLine("Existing User: ", result.Errors.First().Description);
                continue;
            }

            await identityService.AddToRoleAsync(user, role.Name);
        }

        return Result<bool>.Success(true);
    }
}