using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

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
        public async Task<ActionResult<TeamDto>> Create(CreateTeamDto dto)
    {
        var t = new Team { Id = Guid.NewGuid(), Name = dto.Name, Tag = dto.Tag.Trim().ToUpperInvariant(), OwnerUserId = dto.OwnerUserId, LogoUrl = dto.LogoUrl, Bio = dto.Bio, CreatedAt = DateTimeOffset.UtcNow };
        _db.Set<Team>().Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = t.Id }, new TeamDto(t.Id, t.Name, t.Tag, t.OwnerUserId, t.LogoUrl, t.Bio, t.CreatedAt));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamDto dto)
    {
        var t = await _db.Set<Team>().FindAsync(id);
        if (t is null) return NotFound();
        t.Name = dto.Name;
        t.Tag = dto.Tag.Trim().ToUpperInvariant();
        t.LogoUrl = dto.LogoUrl;
        t.Bio = dto.Bio;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var t = await _db.Set<Team>().FindAsync(id);
        if (t is null) return NotFound();
        _db.Set<Team>().Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("/api/teams")] // Custom route to avoid conflict with GetAll
    public async Task<ActionResult<PagedResult<TeamListItemDto>>> GetFiltered(
    [FromQuery] string? search,
    [FromQuery] string? tag,
    [FromQuery] int? minMembers,
    [FromQuery] int? maxMembers,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : (pageSize > 50 ? 50 : pageSize);

        var userId = GetUserIdOrNull();

        var query = _db.Set<Team>()
            .AsNoTracking()
            .AsQueryable();

        // search by name (and optionally tag)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(t => t.Name.Contains(s) || t.Tag.Contains(s));
        }

        // tag = 2–5 char team code (exact match, case-insensitive)
        if (!string.IsNullOrWhiteSpace(tag))
        {
            var code = tag.Trim().ToUpperInvariant();
            query = query.Where(t => t.Tag.ToUpper() == code);
        }

        // Project memberCount + isMember as SQL subqueries
        var projected = query.Select(t => new
        {
            Team = t,
            MemberCount = t.Members.Count(m => m.LeftAt == null),
            IsMember = userId != null && t.Members.Any(m => m.UserId == userId && m.LeftAt == null)
        });

        if (minMembers.HasValue)
            projected = projected.Where(x => x.MemberCount >= minMembers.Value);

        if (maxMembers.HasValue)
            projected = projected.Where(x => x.MemberCount <= maxMembers.Value);

        projected = (sort ?? "").ToLowerInvariant() switch
        {
            "name_asc" => projected.OrderBy(x => x.Team.Name),
            "members_asc" => projected.OrderBy(x => x.MemberCount).ThenBy(x => x.Team.Name),
            "members_desc" => projected.OrderByDescending(x => x.MemberCount).ThenBy(x => x.Team.Name),
            "created_desc" => projected.OrderByDescending(x => x.Team.CreatedAt),
            _ => projected.OrderByDescending(x => x.Team.CreatedAt)
        };

        var total = await projected.CountAsync();

        var items = await projected
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TeamListItemDto(
                x.Team.Id,
                x.Team.Name,
                x.Team.Tag,
                x.Team.OwnerUserId,
                x.Team.LogoUrl,
                x.Team.Bio,
                x.MemberCount,
                x.IsMember
            ))
            .ToListAsync();

        return Ok(new PagedResult<TeamListItemDto>(items, page, pageSize, total));
    }

    private string? GetUserIdOrNull()
    {
        if (User?.Identity?.IsAuthenticated != true) return null;

        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
    }
}