namespace MatchFlow.Domain.Entities;
public sealed class PostLike
{
    public Guid PostId { get; set; }
    public Post? Post { get; set; }

    public string? UserId { get; set; }

    public Guid? TeamId { get; set; }
}