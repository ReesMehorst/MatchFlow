namespace MatchFlow.Domain.Entities;

public sealed class FixtureParticipant
{
    public Guid Id { get; set; }
    public Guid FixtureId { get; set; }
    public Fixture? Fixture { get; set; }

    public Guid TeamId { get; set; }
    public Team? Team { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? InGameName { get; set; }
    public string? Role { get; set; }
    public string? StatsJson { get; set; }
}