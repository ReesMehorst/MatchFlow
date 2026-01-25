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
        var list = await _db.TeamMembers
            .Select(tm => new TeamMemberDto(
                tm.TeamId,
                tm.UserId,
                tm.Role,
                tm.JoinedAt,
                tm.LeftAt
            ))
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{teamId:guid}/{userId}")]
    public async Task<ActionResult<TeamMemberDto>> Get(Guid teamId, string userId)
    {
        var tm = await _db.TeamMembers.FindAsync(teamId, userId);
        if (tm is null) return NotFound();

        return Ok(new TeamMemberDto(
            tm.TeamId,
            tm.UserId,
            tm.Role,
            tm.JoinedAt,
            tm.LeftAt
        ));
    }

    [HttpPost]
    public async Task<ActionResult<TeamMemberDto>> Create(CreateTeamMemberDto dto)
    {
        var exists = await _db.TeamMembers.FindAsync(dto.TeamId, dto.UserId);
        if (exists != null)
            return Conflict("User is already a member of this team.");

        var tm = new TeamMember
        {
            TeamId = dto.TeamId,
            UserId = dto.UserId,
            Role = dto.Role,
            JoinedAt = DateTimeOffset.UtcNow
        };

        _db.TeamMembers.Add(tm);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Get),
            new { teamId = tm.TeamId, userId = tm.UserId },
            new TeamMemberDto(tm.TeamId, tm.UserId, tm.Role, tm.JoinedAt, tm.LeftAt)
        );
    }

    [HttpPut("{teamId:guid}/{userId}")]
    public async Task<IActionResult> Update(Guid teamId, string userId, UpdateTeamMemberDto dto)
    {
        var tm = await _db.TeamMembers.FindAsync(teamId, userId);
        if (tm is null) return NotFound();

        tm.Role = dto.Role;
        tm.LeftAt = dto.LeftAt;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{teamId:guid}/{userId}")]
    public async Task<IActionResult> Delete(Guid teamId, string userId)
    {
        var tm = await _db.TeamMembers.FindAsync(teamId, userId);
        if (tm is null) return NotFound();

        _db.TeamMembers.Remove(tm);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}