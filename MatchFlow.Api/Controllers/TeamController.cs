using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mime;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public TeamController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll()
    {
        var items = await _db.Set<Team>()
            .Select(t => new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamDto>> Get(Guid id)
    {
        var t = await _db.Set<Team>().FindAsync(id);
        if (t is null) return NotFound();
        return Ok(new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<TeamDto>> Create([FromForm] CreateTeamDto dto)
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

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTeamDto dto)
    {
        var t = await _db.Set<Team>().FindAsync(id);
        if (t is null) return NotFound();

        // Basic validation (additional model validation via [Required] on DTO)
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Tag))
            return BadRequest("Name and Tag are required.");

        // Optional: ensure owner is present (either from DTO or from authenticated user)
        if (string.IsNullOrWhiteSpace(dto.OwnerUserId) && User?.Identity?.IsAuthenticated == true)
        {
            // example: get owner id from claims if you prefer that flow
            var uid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(uid))
                dto.OwnerUserId = uid;
        }

        // Handle new logo upload (if provided)
        string? newLogoUrl = null;
        if (dto.LogoFile != null && dto.LogoFile.Length > 0)
        {
            var allowed = new[] { "image/png", "image/jpeg" };
            if (!allowed.Contains(dto.LogoFile.ContentType))
                return BadRequest("Only PNG and JPEG images are allowed for logo.");

            const long maxSize = 5 * 1024 * 1024;
            if (dto.LogoFile.Length > maxSize)
                return BadRequest("Logo file is too large.");

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

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            newLogoUrl = $"{baseUrl}/uploads/teams/{Uri.EscapeDataString(fileName)}";

            // remove old file if it lives in our uploads folder
            if (!string.IsNullOrWhiteSpace(t.LogoUrl) && t.LogoUrl.Contains("/uploads/teams/"))
            {
                try
                {
                    var oldFileName = Path.GetFileName(new Uri(t.LogoUrl).LocalPath);
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "teams", oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
                catch
                {
                    // swallow errors — don't fail the whole request if delete fails.
                }
            }
        }

        // Apply updates
        t.Name = dto.Name;
        t.Tag = dto.Tag.Trim().ToUpperInvariant();
        t.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio;
        if (newLogoUrl != null) t.LogoUrl = newLogoUrl;

        // If you want to enforce OwnerUserId not empty in DB:
        if (!string.IsNullOrWhiteSpace(dto.OwnerUserId))
            t.OwnerUserId = dto.OwnerUserId;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}