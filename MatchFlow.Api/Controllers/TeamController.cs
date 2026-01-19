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
        var t = new Team { Id = Guid.NewGuid(), Name = dto.Name, Tag = dto.Tag, OwnerUserId = dto.OwnerUserId, LogoUrl = dto.LogoUrl, Bio = dto.Bio, CreatedAt = DateTimeOffset.UtcNow };
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
        t.Tag = dto.Tag;
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
}