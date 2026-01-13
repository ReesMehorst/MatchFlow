using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Models;

public sealed class CommentCreateDto
{
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string? AuthorUserId { get; set; }
    public Guid? AuthorTeamId { get; set; } = Guid.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}