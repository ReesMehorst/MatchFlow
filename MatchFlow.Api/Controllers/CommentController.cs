using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public CommentController(MatchFlowDbContext db) => _db = db;

    [HttpGet("post/{postId:guid}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetForPost(Guid postId)
    {
        var list = await _db.Set<Comment>()
            .Where(c => c.PostId == postId)
            .Select(c => new CommentDto(c.Id, c.PostId, c.AuthorUserId, c.AuthorTeamId, c.Body, c.CreatedAt, c.ParentCommentId))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CommentDto>> Get(Guid id)
    {
        var c = await _db.Set<Comment>().FindAsync(id);
        if (c is null) return NotFound();
        return Ok(new CommentDto(c.Id, c.PostId, c.AuthorUserId, c.AuthorTeamId, c.Body, c.CreatedAt, c.ParentCommentId));
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(CreateCommentDto dto)
    {
        var c = new Comment { Id = Guid.NewGuid(), PostId = dto.PostId, AuthorUserId = dto.AuthorUserId, AuthorTeamId = dto.AuthorTeamId, Body = dto.Body, ParentCommentId = dto.ParentCommentId, CreatedAt = DateTimeOffset.UtcNow };
        _db.Set<Comment>().Add(c);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = c.Id }, new CommentDto(c.Id, c.PostId, c.AuthorUserId, c.AuthorTeamId, c.Body, c.CreatedAt, c.ParentCommentId));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCommentDto dto)
    {
        var c = await _db.Set<Comment>().FindAsync(id);
        if (c is null) return NotFound();
        c.Body = dto.Body;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var c = await _db.Set<Comment>().FindAsync(id);
        if (c is null) return NotFound();
        _db.Set<Comment>().Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}