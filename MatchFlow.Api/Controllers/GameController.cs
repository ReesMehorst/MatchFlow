using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public GameController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAll()
    {
        var games = await _db.Set<Game>()
            .Select(g => new GameDto(g.Id, g.Name, g.IconUrl, g.Genre))
            .ToListAsync();
        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameDto>> Get(Guid id)
    {
        var g = await _db.Set<Game>().FindAsync(id);
        if (g is null) return NotFound();
        return Ok(new GameDto(g.Id, g.Name, g.IconUrl, g.Genre));
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> Create(CreateGameDto dto)
    {
        var g = new Game { Id = Guid.NewGuid(), Name = dto.Name, IconUrl = dto.IconUrl, Genre = dto.Genre };
        _db.Set<Game>().Add(g);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = g.Id }, new GameDto(g.Id, g.Name, g.IconUrl, g.Genre));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateGameDto dto)
    {
        var g = await _db.Set<Game>().FindAsync(id);
        if (g is null) return NotFound();
        g.Name = dto.Name;
        g.IconUrl = dto.IconUrl;
        g.Genre = dto.Genre;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var g = await _db.Set<Game>().FindAsync(id);
        if (g is null) return NotFound();
        _db.Set<Game>().Remove(g);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
