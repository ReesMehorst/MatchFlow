using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public PostController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostCreateDto dto)
    {
        var post = new Post
        {
            AuthorUserId    = dto.AuthorUserId,
            AuthorTeamId    = dto.AuthorTeamId,
            GameId          = dto.GameId,
            ContentText     = dto.ContentText,
            CreatedAt       = dto.CreatedAt,
            Visibility      = dto.Visibility
        };

        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }
}