using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatchFlow.Infrastructure.DBContext;
using MatchFlow.Domain.Entities;
using MatchFlow.Api.Models;

namespace MatchFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly MatchFlowDbContext _dbContext;
    public CommentController(MatchFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await _dbContext.Comments
            .AsNoTracking()
            .ToListAsync();

        return Ok(comments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
    {
        var comment = new Comment
        {
            PostId          = dto.PostId,
            ParentCommentId = dto.ParentCommentId,
            AuthorUserId    = dto.AuthorUserId,
            AuthorTeamId    = dto.AuthorTeamId,
            Body            = dto.Body,
            CreatedAt       = dto.CreatedAt
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
    }
}