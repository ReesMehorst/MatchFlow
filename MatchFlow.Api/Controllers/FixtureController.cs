using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixturesController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public FixturesController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FixtureDto>>> GetAll()
    {
        var list = await _db.Set<Fixture>().Select(f =>
            new FixtureDto(f.Id, f.TournamentId, f.GameId, f.TeamAId, f.TeamBId, f.TeamAScore, f.TeamBScore, f.StartAt, f.EndAt, f.Status, f.MatchType, f.Notes, f.CreatedByUserId))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FixtureDto>> Get(Guid id)
    {
        var f = await _db.Set<Fixture>().FindAsync(id);
        if (f is null) return NotFound();
        return Ok(new FixtureDto(f.Id, f.TournamentId, f.GameId, f.TeamAId, f.TeamBId, f.TeamAScore, f.TeamBScore, f.StartAt, f.EndAt, f.Status, f.MatchType, f.Notes, f.CreatedByUserId));
    }

    [HttpPost]
    public async Task<ActionResult<FixtureDto>> Create(CreateFixtureDto dto)
    {
        var f = new Fixture
        {
            Id = Guid.NewGuid(),
            TournamentId = dto.TournamentId,
            GameId = dto.GameId,
            TeamAId = dto.TeamAId,
            TeamBId = dto.TeamBId,
            TeamAScore = dto.TeamAScore,
            TeamBScore = dto.TeamBScore,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Status = dto.Status,
            MatchType = dto.MatchType,
            Notes = dto.Notes,
            CreatedByUserId = dto.CreatedByUserId
        };
        _db.Set<Fixture>().Add(f);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = f.Id }, new FixtureDto(f.Id, f.TournamentId, f.GameId, f.TeamAId, f.TeamBId, f.TeamAScore, f.TeamBScore, f.StartAt, f.EndAt, f.Status, f.MatchType, f.Notes, f.CreatedByUserId));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateFixtureDto dto)
    {
        var f = await _db.Set<Fixture>().FindAsync(id);
        if (f is null) return NotFound();
        f.TournamentId = dto.TournamentId;
        f.GameId = dto.GameId;
        f.TeamAId = dto.TeamAId;
        f.TeamBId = dto.TeamBId;
        f.TeamAScore = dto.TeamAScore;
        f.TeamBScore = dto.TeamBScore;
        f.StartAt = dto.StartAt;
        f.EndAt = dto.EndAt;
        f.Status = dto.Status;
        f.MatchType = dto.MatchType;
        f.Notes = dto.Notes;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var f = await _db.Set<Fixture>().FindAsync(id);
        if (f is null) return NotFound();
        _db.Set<Fixture>().Remove(f);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}