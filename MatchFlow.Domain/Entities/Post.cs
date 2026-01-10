namespace MatchFlow.Domain.Entities;

public enum Visibility { Public, TeamOnly, Private }

public sealed class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? AuthorUserId { get; set; }

    public Guid? AuthorTeamId { get; set; }
    public Team? AuthorTeam { get; set; }

    public Guid? GameId { get; set; }
    public Game? Game { get; set; }

    public string? ContentText { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Visibility Visibility { get; set; } = Visibility.Public;

    public ICollection<PostMedia> Media { get; set; } = [];
   // public ICollection<PostAchievement> PostAchievements { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}