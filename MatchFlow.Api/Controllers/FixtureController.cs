using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixtureController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public FixtureController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fixtures = await _dbContext.Fixtures
            .AsNoTracking()
            .ToListAsync();

        return Ok(fixtures);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var fixture = await _dbContext.Fixtures.FindAsync(id);
        if (fixture == null) return NotFound();
        return Ok(fixture);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FixtureCreateDto dto)
    {
        var fixture = new Fixture
        {
            GameId          = dto.GameId,
            TournamentId    = dto.TournamentId,
            StartAt         = dto.StartAt,
            EndAt           = dto.EndAt,
            TeamAId         = dto.TeamAId,
            TeamBId         = dto.TeamBId,
            TeamAScore      = dto.TeamAScore,
            TeamBScore      = dto.TeamBScore,
            MatchType       = dto.MatchType,
            Status          = dto.Status,
            CreatedByUserId = dto.CreatedByUserId,
            Notes           = dto.Notes
        };

        _dbContext.Fixtures.Add(fixture);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = fixture.Id }, fixture);
    }
}