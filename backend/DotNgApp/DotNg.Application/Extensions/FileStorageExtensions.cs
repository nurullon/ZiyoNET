using Microsoft.AspNetCore.Http;

namespace DotNg.Application.Extensions;

public static class FileStorageExtensions
{
    private static string GetWebRootPath(string folderName) =>
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

    public static async Task<string> SaveUserImageAsync(this IFormFile signatureImage, string? fileName = null)
    {
        var webrootPath = GetWebRootPath("UserImages");
        if (!Directory.Exists(webrootPath))
            Directory.CreateDirectory(webrootPath);

        fileName ??= $"{Guid.NewGuid():N}.jpg";
        var fileWithPath = Path.Combine(webrootPath, fileName);

        using (var fileStream = new FileStream(fileWithPath, FileMode.Create))
            await signatureImage.CopyToAsync(fileStream);

        return $"UserImages/{fileName}";
    }

    public static void DeleteUserImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        var webrootPath = GetWebRootPath("UserImages");
        var filePath = Path.Combine(webrootPath, Path.GetFileName(imageUrl));
        if (File.Exists(filePath))
        {
                File.Delete(filePath);
        }
    }
}