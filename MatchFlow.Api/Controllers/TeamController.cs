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
    private readonly MatchFlow.Api.Services.IUploadService _uploads;
    private readonly MatchFlow.Api.Services.ICurrentUserService _currentUser;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

    public TeamController(MatchFlowDbContext db, MatchFlow.Api.Services.IUploadService uploads, MatchFlow.Api.Services.ICurrentUserService currentUser, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        => (_db, _uploads, _currentUser, _env) = (db, uploads, currentUser, env);

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
    public async Task<ActionResult<TeamDto>> Create([FromForm] CreateTeamDto dto, CancellationToken cancellationToken)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Tag))
            return BadRequest("Name and Tag are required.");

        // Resolve owner id from DTO or current user claims
        if (string.IsNullOrWhiteSpace(dto.OwnerUserId))
            dto.OwnerUserId = _currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(dto.OwnerUserId))
            return BadRequest("OwnerUserId is required.");

        string? logoUrl = null;
        try
        {
            logoUrl = await _uploads.SaveTeamLogoAsync(dto.LogoFile, cancellationToken);
            if (logoUrl != null)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                logoUrl = baseUrl + logoUrl;
            }
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }

        var t = new MatchFlow.Domain.Entities.Team
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Tag = dto.Tag.Trim().ToUpperInvariant(),
            OwnerUserId = dto.OwnerUserId!,
            LogoUrl = logoUrl,
            Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Set<MatchFlow.Domain.Entities.Team>().Add(t);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = t.Id }, new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt));
    }

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTeamDto dto, CancellationToken cancellationToken)
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
            try
            {
                var saved = await _uploads.SaveTeamLogoAsync(dto.LogoFile, cancellationToken);
                if (saved != null)
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    newLogoUrl = baseUrl + saved;

                    // remove old file if it lives in our uploads folder
                    if (!string.IsNullOrWhiteSpace(t.LogoUrl) && t.LogoUrl.Contains("/uploads/teams/"))
                    {
                        try
                        {
                            var oldFileName = Path.GetFileName(new Uri(t.LogoUrl).LocalPath);
                            var oldFilePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "teams", oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                                System.IO.File.Delete(oldFilePath);
                        }
                        catch
                        {
                            // swallow errors — don't fail the whole request if delete fails.
                        }
                    }
                }
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
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

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}