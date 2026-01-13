using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Models;

public sealed class PostCreateDto
{
    public string? AuthorUserId { get; set; }
    public Guid? AuthorTeamId { get; set; }
    public Guid? GameId { get; set; }
    public string? ContentText { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Visibility Visibility { get; set; } = Visibility.Public;
}