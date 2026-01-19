using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public PostController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetAll()
    {
        var list = await _db.Set<Post>()
            .Select(p => new PostDto(p.Id, p.GameId, p.AuthorTeamId, p.AuthorUserId, p.ContentText, p.CreatedAt, p.Visibility))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostDto>> Get(Guid id)
    {
        var p = await _db.Set<Post>().FindAsync(id);
        if (p is null) return NotFound();
        return Ok(new PostDto(p.Id, p.GameId, p.AuthorTeamId, p.AuthorUserId, p.ContentText, p.CreatedAt, p.Visibility));
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create(CreatePostDto dto)
    {
        var p = new Post { Id = Guid.NewGuid(), GameId = dto.GameId, AuthorTeamId = dto.AuthorTeamId, AuthorUserId = dto.AuthorUserId, ContentText = dto.ContentText, Visibility = dto.Visibility, CreatedAt = DateTimeOffset.UtcNow };
        _db.Set<Post>().Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = p.Id }, new PostDto(p.Id, p.GameId, p.AuthorTeamId, p.AuthorUserId, p.ContentText, p.CreatedAt, p.Visibility));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePostDto dto)
    {
        var p = await _db.Set<Post>().FindAsync(id);
        if (p is null) return NotFound();
        p.ContentText = dto.ContentText;
        p.Visibility = dto.Visibility;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var p = await _db.Set<Post>().FindAsync(id);
        if (p is null) return NotFound();
        _db.Set<Post>().Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}