using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamGameController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public TeamGameController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamGameDto>>> GetAll()
    {
        var list = await _db.Set<TeamGame>()
            .Select(tg => new TeamGameDto(tg.TeamId, tg.GameId, tg.Primary))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{teamId:guid}/{gameId:guid}")]
    public async Task<ActionResult<TeamGameDto>> Get(Guid teamId, Guid gameId)
    {
        var tg = await _db.Set<TeamGame>().FindAsync(teamId, gameId);
        if (tg is null) return NotFound();
        return Ok(new TeamGameDto(tg.TeamId, tg.GameId, tg.Primary));
    }

    [HttpPost]
    public async Task<ActionResult<TeamGameDto>> Create(CreateTeamGameDto dto)
    {
        var tg = new TeamGame
        {
            TeamId = dto.TeamId,
            GameId = dto.GameId,
            Primary = dto.Primary
        };
        _db.Set<TeamGame>().Add(tg);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { teamId = tg.TeamId, gameId = tg.GameId }, new TeamGameDto(tg.TeamId, tg.GameId, tg.Primary));
    }

    [HttpPut("{teamId:guid}/{gameId:guid}")]
    public async Task<IActionResult> Update(Guid teamId, Guid gameId, UpdateTeamGameDto dto)
    {
        var tg = await _db.Set<TeamGame>().FindAsync(teamId, gameId);
        if (tg is null) return NotFound();
        tg.Primary = dto.Primary;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{teamId:guid}/{gameId:guid}")]
    public async Task<IActionResult> Delete(Guid teamId, Guid gameId)
    {
        var tg = await _db.Set<TeamGame>().FindAsync(teamId, gameId);
        if (tg is null) return NotFound();
        _db.Set<TeamGame>().Remove(tg);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}