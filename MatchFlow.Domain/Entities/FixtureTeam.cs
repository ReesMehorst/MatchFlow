namespace MatchFlow.Domain.Entities;

public enum FixtureSide { A = 1, B = 2 }

public sealed class FixtureTeam
{
    public Guid FixtureId { get; set; }
    public Fixture? Fixture { get; set; }

    public Guid TeamId { get; set; }
    public Team? Team { get; set; }

    public FixtureSide Side { get; set; }

    public int? Score { get; set; }
    public string? Result { get; set; } // Win/Loss/Draw
}