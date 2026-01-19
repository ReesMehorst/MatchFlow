using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Dtos;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostLikeController : ControllerBase
{
    private readonly MatchFlowDbContext _db;

    public PostLikeController(MatchFlowDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostLikeDto>>> GetAll()
    {
        var list = await _db.Set<PostLike>()
            .Select(pl => new PostLikeDto(pl.PostId, pl.UserId, pl.TeamId))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{postId:guid}/{userId}/{teamId:guid}")]
    public async Task<ActionResult<PostLikeDto>> Get(Guid postId, string userId, Guid teamId)
    {
        var pl = await _db.Set<PostLike>().FindAsync(postId, userId, teamId);
        if (pl is null) return NotFound();
        return Ok(new PostLikeDto(pl.PostId, pl.UserId, pl.TeamId));
    }

    [HttpPost]
    public async Task<ActionResult<PostLikeDto>> Create(CreatePostLikeDto dto)
    {
        var pl = new PostLike
        {
            PostId = dto.PostId,
            UserId = dto.UserId,
            TeamId = dto.TeamId
        };
        _db.Set<PostLike>().Add(pl);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { postId = pl.PostId, userId = pl.UserId, teamId = pl.TeamId }, new PostLikeDto(pl.PostId, pl.UserId, pl.TeamId));
    }

    [HttpDelete("{postId:guid}/{userId}/{teamId:guid}")]
    public async Task<IActionResult> Delete(Guid postId, string userId, Guid teamId)
    {
        var pl = await _db.Set<PostLike>().FindAsync(postId, userId, teamId);
        if (pl is null) return NotFound();
        _db.Set<PostLike>().Remove(pl);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}