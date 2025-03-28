using DotNg.Application.Serialization;
using DotNg.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNg.API.Controllers;

[Authorize]
[ApiController]
[Route("api/excel")]
public class ExcelController(IExcelService excelService,
    ResponseSerializer responseSerializer) : Controller
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadUsers(IFormFile file)
    {
        return responseSerializer.ToActionResult(await excelService.UploadUsersAsync(file));
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadExcel()
    {
        return responseSerializer.ToActionResult(await excelService.ExportUsersToExcelAsync());
    }
}