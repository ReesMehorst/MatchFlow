using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public TournamentController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetAll()
    {
        var list = await _db.Set<Tournament>().Select(t => new TournamentDto(t.Id, t.Name)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TournamentDto>> Get(Guid id)
    {
        var t = await _db.Set<Tournament>().FindAsync(id);
        if (t is null) return NotFound();
        return Ok(new TournamentDto(t.Id, t.Name));
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDto>> Create(CreateTournamentDto dto)
    {
        var t = new Tournament { Id = Guid.NewGuid(), Name = dto.Name };
        _db.Set<Tournament>().Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = t.Id }, new TournamentDto(t.Id, t.Name));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTournamentDto dto)
    {
        var t = await _db.Set<Tournament>().FindAsync(id);
        if (t is null) return NotFound();
        t.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var t = await _db.Set<Tournament>().FindAsync(id);
        if (t is null) return NotFound();
        _db.Set<Tournament>().Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}