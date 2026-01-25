using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MatchFlow.Api.Services;

public class UploadService : IUploadService
{
    private readonly IWebHostEnvironment _env;
    public UploadService(IWebHostEnvironment env) => _env = env;

    public async Task<string?> SaveTeamLogoAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0) return null;

        var allowed = new[] { "image/png", "image/jpeg" };
        if (!allowed.Contains(file.ContentType))
            throw new ArgumentException("Only PNG and JPEG images are allowed for logo.");

        const long maxSize = 5 * 1024 * 1024;
        if (file.Length > maxSize)
            throw new ArgumentException("Logo file is too large.");

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "teams");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext))
        {
            ext = file.ContentType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                _ => ".img"
            };
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // return path relative to webroot (no host) so callers can combine with request host if needed
        return $"/uploads/teams/{Uri.EscapeDataString(fileName)}";
    }
}
