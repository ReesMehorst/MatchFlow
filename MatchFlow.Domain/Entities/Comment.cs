namespace MatchFlow.Domain.Entities;
public sealed class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Post? Post { get; set; }

    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }

    public string? AuthorUserId { get; set; }

    public Guid? AuthorTeamId { get; set; }
    public Team? AuthorTeam { get; set; }

    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}