using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public TeamController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _dbContext.Teams
            .AsNoTracking()
            .ToListAsync();

        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var team = await _dbContext.Teams.FindAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Team team)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }
}
