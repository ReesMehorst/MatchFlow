using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixtureTeamController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public FixtureTeamController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fixtureTeams = await _dbContext.FixtureTeams
            .AsNoTracking()
            .ToListAsync();

        return Ok(fixtureTeams);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var fixtureTeams = await _dbContext.FixtureTeams.FindAsync(id);
        if (fixtureTeams == null) return NotFound();
        return Ok(fixtureTeams);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FixtureTeamCreateDto dto)
    {
        var fixtureTeam = new FixtureTeam
        {
            FixtureId   = dto.FixtureId,
            TeamId      = dto.TeamId,
            Side        = dto.Side,
            Score       = dto.Score,
            Result      = dto.Result
        };

        _dbContext.FixtureTeams.Add(fixtureTeam);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = fixtureTeam.Id }, fixtureTeam);
    }
}