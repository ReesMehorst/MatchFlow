using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixtureParticipantController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public FixtureParticipantController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixtureParticipantDto>>> GetAll()
    {
        var list = await _db.Set<FixtureParticipant>()
            .Select(fp => new FixtureParticipantDto(fp.FixtureId, fp.UserId, fp.TeamId, fp.InGameName, fp.Role, fp.StatsJson))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{fixtureId:guid}/{userId}")]
    public async Task<ActionResult<FixtureParticipantDto>> Get(Guid fixtureId, string userId)
    {
        var fp = await _db.Set<FixtureParticipant>().FindAsync(fixtureId, userId);
        if (fp is null) return NotFound();
        return Ok(new FixtureParticipantDto(fp.FixtureId, fp.UserId, fp.TeamId, fp.InGameName, fp.Role, fp.StatsJson));
    }

    [HttpPost]
    public async Task<ActionResult<FixtureParticipantDto>> Create(CreateFixtureParticipantDto dto)
    {
        var fp = new FixtureParticipant
        {
            FixtureId = dto.FixtureId,
            UserId = dto.UserId,
            TeamId = dto.TeamId,
            InGameName = dto.InGameName,
            Role = dto.Role,
            StatsJson = dto.StatsJson
        };
        _db.Set<FixtureParticipant>().Add(fp);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { fixtureId = fp.FixtureId, userId = fp.UserId }, new FixtureParticipantDto(fp.FixtureId, fp.UserId, fp.TeamId, fp.InGameName, fp.Role, fp.StatsJson));
    }

    [HttpPut("{fixtureId:guid}/{userId}")]
    public async Task<IActionResult> Update(Guid fixtureId, string userId, UpdateFixtureParticipantDto dto)
    {
        var fp = await _db.Set<FixtureParticipant>().FindAsync(fixtureId, userId);
        if (fp is null) return NotFound();
        fp.TeamId = dto.TeamId;
        fp.InGameName = dto.InGameName;
        fp.Role = dto.Role;
        fp.StatsJson = dto.StatsJson;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{fixtureId:guid}/{userId}")]
    public async Task<IActionResult> Delete(Guid fixtureId, string userId)
    {
        var fp = await _db.Set<FixtureParticipant>().FindAsync(fixtureId, userId);
        if (fp is null) return NotFound();
        _db.Set<FixtureParticipant>().Remove(fp);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}