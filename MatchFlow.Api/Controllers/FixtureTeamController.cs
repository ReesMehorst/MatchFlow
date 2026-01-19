using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixtureTeamController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public FixtureTeamController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixtureTeamDto>>> GetAll()
    {
        var list = await _db.Set<FixtureTeam>()
            .Select(ft => new FixtureTeamDto(ft.FixtureId, ft.TeamId, ft.Side, ft.Score, ft.Result))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{fixtureId:guid}/{teamId:guid}")]
    public async Task<ActionResult<FixtureTeamDto>> Get(Guid fixtureId, Guid teamId)
    {
        var ft = await _db.Set<FixtureTeam>().FindAsync(fixtureId, teamId);
        if (ft is null) return NotFound();
        return Ok(new FixtureTeamDto(ft.FixtureId, ft.TeamId, ft.Side, ft.Score, ft.Result));
    }

    [HttpPost]
    public async Task<ActionResult<FixtureTeamDto>> Create(CreateFixtureTeamDto dto)
    {
        var ft = new FixtureTeam
        {
            FixtureId = dto.FixtureId,
            TeamId = dto.TeamId,
            Side = dto.Side,
            Score = dto.Score,
            Result = dto.Result
        };
        _db.Set<FixtureTeam>().Add(ft);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { fixtureId = ft.FixtureId, teamId = ft.TeamId }, new FixtureTeamDto(ft.FixtureId, ft.TeamId, ft.Side, ft.Score, ft.Result));
    }

    [HttpPut("{fixtureId:guid}/{teamId:guid}")]
    public async Task<IActionResult> Update(Guid fixtureId, Guid teamId, UpdateFixtureTeamDto dto)
    {
        var ft = await _db.Set<FixtureTeam>().FindAsync(fixtureId, teamId);
        if (ft is null) return NotFound();
        ft.Side = dto.Side;
        ft.Score = dto.Score;
        ft.Result = dto.Result;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{fixtureId:guid}/{teamId:guid}")]
    public async Task<IActionResult> Delete(Guid fixtureId, Guid teamId)
    {
        var ft = await _db.Set<FixtureTeam>().FindAsync(fixtureId, teamId);
        if (ft is null) return NotFound();
        _db.Set<FixtureTeam>().Remove(ft);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}