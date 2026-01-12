using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public GameController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _dbContext.Games
            .AsNoTracking()
            .ToListAsync();

        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _dbContext.Games.FindAsync(id);
        if (game == null) return NotFound();
        return Ok(game);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GameCreateDto dto)
    {
        var game = new Game
        {
            Name = dto.Name,
            Genre = dto.Genre,
            IconUrl = dto.IconUrl
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }
}
