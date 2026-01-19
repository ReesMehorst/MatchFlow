using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMemberController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public TeamMemberController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamMemberDto>>> GetAll()
    {
        var list = await _db.Set<TeamMember>()
            .Select(tm => new TeamMemberDto(tm.Id, tm.TeamId, tm.UserId, tm.Role, tm.JoinedAt, tm.LeftAt))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeamMemberDto>> Get(Guid id)
    {
        var tm = await _db.Set<TeamMember>().FindAsync(id);
        if (tm is null) return NotFound();
        return Ok(new TeamMemberDto(tm.Id, tm.TeamId, tm.UserId, tm.Role, tm.JoinedAt, tm.LeftAt));
    }

    [HttpPost]
    public async Task<ActionResult<TeamMemberDto>> Create(CreateTeamMemberDto dto)
    {
        var tm = new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = dto.TeamId,
            UserId = dto.UserId,
            Role = dto.Role,
            JoinedAt = DateTimeOffset.UtcNow
        };
        _db.Set<TeamMember>().Add(tm);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = tm.Id }, new TeamMemberDto(tm.Id, tm.TeamId, tm.UserId, tm.Role, tm.JoinedAt, tm.LeftAt));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTeamMemberDto dto)
    {
        var tm = await _db.Set<TeamMember>().FindAsync(id);
        if (tm is null) return NotFound();
        tm.Role = dto.Role;
        tm.LeftAt = dto.LeftAt;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tm = await _db.Set<TeamMember>().FindAsync(id);
        if (tm is null) return NotFound();
        _db.Set<TeamMember>().Remove(tm);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}