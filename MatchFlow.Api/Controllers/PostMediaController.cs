using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostMediaController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public PostMediaController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostMediaDto>>> GetAll()
    {
        var list = await _db.Set<PostMedia>()
            .Select(pm => new PostMediaDto(pm.Id, pm.PostId, pm.Url, pm.ThumbnailUrl, pm.MediaType, pm.DurationSec, pm.CreatedAt))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostMediaDto>> Get(Guid id)
    {
        var pm = await _db.Set<PostMedia>().FindAsync(id);
        if (pm is null) return NotFound();
        return Ok(new PostMediaDto(pm.Id, pm.PostId, pm.Url, pm.ThumbnailUrl, pm.MediaType, pm.DurationSec, pm.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<PostMediaDto>> Create(CreatePostMediaDto dto)
    {
        var pm = new PostMedia
        {
            Id = Guid.NewGuid(),
            PostId = dto.PostId,
            Url = dto.Url,
            ThumbnailUrl = dto.ThumbnailUrl,
            MediaType = dto.MediaType,
            DurationSec = dto.DurationSec,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Set<PostMedia>().Add(pm);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = pm.Id }, new PostMediaDto(pm.Id, pm.PostId, pm.Url, pm.ThumbnailUrl, pm.MediaType, pm.DurationSec, pm.CreatedAt));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePostMediaDto dto)
    {
        var pm = await _db.Set<PostMedia>().FindAsync(id);
        if (pm is null) return NotFound();
        pm.ThumbnailUrl = dto.ThumbnailUrl;
        pm.DurationSec = dto.DurationSec;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var pm = await _db.Set<PostMedia>().FindAsync(id);
        if (pm is null) return NotFound();
        _db.Set<PostMedia>().Remove(pm);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}