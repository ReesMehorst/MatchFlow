using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserGameController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public UserGameController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserGameDto>>> GetAll()
    {
        var list = await _db.Set<UserGame>()
            .Select(ug => new UserGameDto(ug.UserId, ug.GameId, ug.RankElo))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{userId}/{gameId:guid}")]
    public async Task<ActionResult<UserGameDto>> Get(string userId, Guid gameId)
    {
        var ug = await _db.Set<UserGame>().FindAsync(userId, gameId);
        if (ug is null) return NotFound();
        return Ok(new UserGameDto(ug.UserId, ug.GameId, ug.RankElo));
    }

    [HttpPost]
    public async Task<ActionResult<UserGameDto>> Create(CreateUserGameDto dto)
    {
        var ug = new UserGame
        {
            UserId = dto.UserId,
            GameId = dto.GameId,
            RankElo = dto.RankElo
        };
        _db.Set<UserGame>().Add(ug);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { userId = ug.UserId, gameId = ug.GameId }, new UserGameDto(ug.UserId, ug.GameId, ug.RankElo));
    }

    [HttpPut("{userId}/{gameId:guid}")]
    public async Task<IActionResult> Update(string userId, Guid gameId, UpdateUserGameDto dto)
    {
        var ug = await _db.Set<UserGame>().FindAsync(userId, gameId);
        if (ug is null) return NotFound();
        ug.RankElo = dto.RankElo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{userId}/{gameId:guid}")]
    public async Task<IActionResult> Delete(string userId, Guid gameId)
    {
        var ug = await _db.Set<UserGame>().FindAsync(userId, gameId);
        if (ug is null) return NotFound();
        _db.Set<UserGame>().Remove(ug);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}