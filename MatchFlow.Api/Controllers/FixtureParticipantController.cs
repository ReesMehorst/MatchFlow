using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FixtureParticipantController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public FixtureParticipantController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fixtureParticipants = await _dbContext.FixtureParticipants
            .AsNoTracking()
            .ToListAsync();

        return Ok(fixtureParticipants);
    }

    [HttpGet("{fixtureId:guid}/{userId:string}")]
    public async Task<IActionResult> GetById(Guid fixtureId, string userId)
    {
        var fixtureParticipant = await _dbContext.FixtureParticipants.FindAsync(fixtureId, userId);
        if (fixtureParticipant == null) return NotFound();
        return Ok(fixtureParticipant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FixtureParticipantCreateDto dto)
    {
        var fixtureParticipant = new FixtureParticipant
        {
            FixtureId   = dto.FixtureId,
            TeamId      = dto.TeamId,
            UserId      = dto.UserId,
            InGameName  = dto.InGameName,
            Role        = dto.Role,
            StatsJson   = dto.StatsJson
        };

        _dbContext.FixtureParticipants.Add(fixtureParticipant);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(
            nameof(GetById),
            new { fixtureId = fixtureParticipant.FixtureId, userId = fixtureParticipant.UserId },
            fixtureParticipant);
    }
}