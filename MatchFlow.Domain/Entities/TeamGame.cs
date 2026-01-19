namespace MatchFlow.Domain.Entities;

public sealed class TeamGame
{
    public Guid TeamId { get; set; }
    public Team? Team { get; set; }

    public Guid GameId { get; set; }
    public Game? Game { get; set; }

    public bool Primary { get; set; }
}
