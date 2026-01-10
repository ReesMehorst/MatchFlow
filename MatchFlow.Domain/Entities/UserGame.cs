namespace MatchFlow.Domain.Entities;

public sealed class UserGame
{
    public string UserId { get; set; } = string.Empty;

    public Guid GameId { get; set; }
    public Game? Game { get; set; }

    public int? RankElo { get; set; }
}