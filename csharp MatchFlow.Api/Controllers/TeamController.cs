// Replace the existing Create(CreateTeamDto dto) method with this one (or add as an overload).
// Ensure the file system has a wwwroot/uploads/teams folder and that static files are served.
using MatchFlow.Api.Dtos;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mime;

[HttpPost]
[Consumes("multipart/form-data")]
public async Task<ActionResult<TeamDto>> Create([FromForm] CreateTeamFormDto dto)
{
    // Basic validation
    if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Tag))
        return BadRequest("Name and Tag are required.");

    string? logoUrl = null;

    if (dto.LogoFile != null && dto.LogoFile.Length > 0)
    {
        // Validate allowed file types
        var allowed = new[] { "image/png", "image/jpeg" };
        if (!allowed.Contains(dto.LogoFile.ContentType))
            return BadRequest("Only PNG and JPEG images are allowed for logo.");

        // Optional: size check (e.g. 5MB)
        const long maxSize = 5 * 1024 * 1024;
        if (dto.LogoFile.Length > maxSize)
            return BadRequest("Logo file is too large.");

        // Build a safe filename and save to wwwroot/uploads/teams
        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "teams");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(dto.LogoFile.FileName);
        if (string.IsNullOrEmpty(ext))
        {
            ext = dto.LogoFile.ContentType switch
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
            await dto.LogoFile.CopyToAsync(stream);
        }

        // Public URL (ensure your app serves static files from wwwroot)
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        logoUrl = $"{baseUrl}/uploads/teams/{Uri.EscapeDataString(fileName)}";
    }

    var t = new MatchFlow.Domain.Entities.Team
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Tag = dto.Tag.Trim().ToUpperInvariant(),
        OwnerUserId = dto.OwnerUserId,
        LogoUrl = logoUrl,
        Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio,
        CreatedAt = DateTimeOffset.UtcNow
    };

    _db.Set<MatchFlow.Domain.Entities.Team>().Add(t);
    await _db.SaveChangesAsync();
    return CreatedAtAction(nameof(Get), new { id = t.Id }, new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt));
}