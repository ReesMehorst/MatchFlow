using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ILogger<TeamController> _logger;

    public TeamController(MatchFlowDbContext db, MatchFlow.Api.Services.IUploadService uploads, MatchFlow.Api.Services.ICurrentUserService currentUser, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, ILogger<TeamController> logger)
        => (_db, _uploads, _currentUser, _env, _logger) = (db, uploads, currentUser, env, logger);

    [HttpGet]
    public async Task<ActionResult<PagedResult<TeamListItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? tag,
        [FromQuery] int? minMembers,
        [FromQuery] int? maxMembers,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // normalize
        sort ??= "members_desc";
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var currentUserId = _currentUser.GetUserId();

        // Use a teams query and apply filters directly on the Team entity
        var teamsQ = _db.Teams.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            teamsQ = teamsQ.Where(t => EF.Functions.Like(t.Name, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            teamsQ = teamsQ.Where(t => EF.Functions.Like(t.Tag, tag));
        }

        if (minMembers.HasValue || maxMembers.HasValue)
        {
            // LATER
        }

        var membersActive = _db.TeamMembers.Where(tm => tm.LeftAt == null);

        var baseQ = teamsQ
            .GroupJoin(membersActive, t => t.Id, tm => tm.TeamId, (t, members) => new { t, members })
            .Select(x => new
            {
                x.t.Id,
                x.t.Name,
                x.t.Tag,
                x.t.OwnerUserId,
                x.t.LogoUrl,
                x.t.Bio,
                x.t.CreatedAt,
                MemberCount = x.members.Count(),
                IsMember = currentUserId != null && x.members.Any(m => m.UserId == currentUserId)
            }).AsQueryable();

        if (minMembers.HasValue)
            baseQ = baseQ.Where(x => x.MemberCount >= minMembers.Value);
        if (maxMembers.HasValue)
            baseQ = baseQ.Where(x => x.MemberCount <= maxMembers.Value);

        var beforeCount = await baseQ.CountAsync();
        _logger.LogInformation("Team listing: total teams before filter = {Count}", beforeCount);

        var q = baseQ;

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(x => EF.Functions.Like(x.Name, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            q = q.Where(x => EF.Functions.Like(x.Tag, tag));
        }

        if (minMembers.HasValue)
            q = q.Where(x => x.MemberCount >= minMembers.Value);

        if (maxMembers.HasValue)
            q = q.Where(x => x.MemberCount <= maxMembers.Value);

        q = sort switch
        {
            "members_asc" => q.OrderBy(x => x.MemberCount).ThenBy(x => x.Name),
            "name_asc" => q.OrderBy(x => x.Name),
            "created_desc" => q.OrderByDescending(x => x.CreatedAt),
            _ => q.OrderByDescending(x => x.MemberCount).ThenBy(x => x.Name),
        };

        var total = await q.CountAsync();
        _logger.LogInformation("Team listing: total after filters = {Count}", total);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new TeamListItemDto(x.Id, x.Name, x.Tag, x.OwnerUserId, x.LogoUrl, x.Bio, x.MemberCount, x.IsMember))
            .ToListAsync();

        var paged = new PagedResult<TeamListItemDto>(items, page, pageSize, total);
        return Ok(paged);
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

        // Ensure the owner is also added as a team member so member counts include owner
        var ownerMember = new TeamMember
        {
            TeamId = t.Id,
            UserId = t.OwnerUserId,
            Role = "Owner",
            JoinedAt = DateTimeOffset.UtcNow
        };
        _db.TeamMembers.Add(ownerMember);

        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = t.Id }, new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt));
    }

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTeamDto dto, CancellationToken cancellationToken)
    {
        var t = await _db.Set<Team>().FindAsync(id);
        if (t is null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Tag))
            return BadRequest("Name and Tag are required.");

        // ensure owner is present (either from DTO or from authenticated user)
        if (string.IsNullOrWhiteSpace(dto.OwnerUserId) && User?.Identity?.IsAuthenticated == true)
        {
            // get owner id from claims if you prefer that flow
            var uid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(uid))
                dto.OwnerUserId = uid;
        }

        // Handle new logo upload
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
                            // swallow errors
                        }
                    }
                }
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        t.Name = dto.Name;
        t.Tag = dto.Tag.Trim().ToUpperInvariant();
        t.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio;
        if (newLogoUrl != null) t.LogoUrl = newLogoUrl;

        if (!string.IsNullOrWhiteSpace(dto.OwnerUserId))
            t.OwnerUserId = dto.OwnerUserId;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("list")]
    public async Task<ActionResult<PagedResult<TeamListItemDto>>> ListAll()
    {
        var currentUserId = _currentUser.GetUserId();

        var items = await _db.Teams
            .Select(t => new TeamListItemDto(
                t.Id,
                t.Name,
                t.Tag,
                t.OwnerUserId,
                t.LogoUrl,
                t.Bio,
                _db.TeamMembers.Count(tm => tm.TeamId == t.Id && tm.LeftAt == null),
                currentUserId != null && _db.TeamMembers.Any(tm => tm.TeamId == t.Id && tm.UserId == currentUserId && tm.LeftAt == null)
            ))
            .ToListAsync();

        _logger.LogInformation("ListAll: returning {Count} teams: {Names}", items.Count, items.Select(i => i.Name).ToArray());

        var paged = new PagedResult<TeamListItemDto>(items, 1, items.Count, items.Count);
        return Ok(paged);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<IEnumerable<TeamMemberDto>>> GetMembers(Guid id)
    {
        var members = await _db.Set<TeamMember>()
            .Where(m => m.TeamId == id)
            .Select(tm => new TeamMemberDto(
                tm.TeamId, 
                tm.UserId, 
                tm.Role, 
                tm.JoinedAt, 
                tm.LeftAt
            ))
            .ToListAsync();
        return Ok(members);
    }

    [Authorize]
    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinTeam(Guid id)
    {
        var userId = _currentUser.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        // ensure team exists
        var team = await _db.Set<Team>().FindAsync(id);
        if (team is null) return NotFound();

        // Prevent duplicate active membership
        var existing = await _db.TeamMembers.FirstOrDefaultAsync(tm => tm.TeamId == id && tm.UserId == userId && tm.LeftAt == null);
        if (existing != null) return Conflict("Already a member");

        var member = new TeamMember
        {
            TeamId = id,
            UserId = userId,
            Role = "Member",
            JoinedAt = DateTimeOffset.UtcNow
        };

        _db.TeamMembers.Add(member);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{id:guid}/matches")]
    public async Task<ActionResult<IEnumerable<FixtureDto>>> GetTeamFixtures(Guid id)
    {
        var fixtures = await _db.Set<Fixture>()
            .Where(f => f.TeamAId == id || f.TeamBId == id)
            .Select(f => new FixtureDto(
                f.Id, 
                f.TournamentId,
                f.GameId,
                f.TeamAId,
                f.TeamBId,
                f.TeamAScore, 
                f.TeamBScore, 
                f.StartAt,
                f.EndAt,
                f.Status,
                f.MatchType,
                f.Notes,
                f.CreatedByUserId
            ))
            .ToListAsync();
        return Ok(fixtures);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var t = await _db.Set<Team>().FindAsync(new object[] { id }, cancellationToken);
        if (t is null) return NotFound();

        // Only allow owner or admin to delete a team
        var userId = _currentUser.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
        if (!string.Equals(t.OwnerUserId, userId, StringComparison.OrdinalIgnoreCase) && !User.IsInRole("Admin"))
            return Forbid();

        // Remove dependent entities to avoid FK issues
        var members = _db.TeamMembers.Where(tm => tm.TeamId == id);
        _db.TeamMembers.RemoveRange(members);

        var fixtures = _db.Set<Fixture>().Where(f => f.TeamAId == id || f.TeamBId == id);
        _db.Set<Fixture>().RemoveRange(fixtures);

        // Remove uploaded logo file in our uploads folder
        if (!string.IsNullOrWhiteSpace(t.LogoUrl) && t.LogoUrl.Contains("/uploads/teams/"))
        {
            try
            {
                var fileName = Path.GetFileName(new Uri(t.LogoUrl).LocalPath);
                var filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "teams", fileName);
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
            catch
            {
                // swallow error if it fails
            }
        }

        _db.Set<Team>().Remove(t);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}