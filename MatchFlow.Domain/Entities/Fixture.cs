namespace MatchFlow.Domain.Entities;

public enum MatchType { Scrim, Tournament, Ranked, Friendly }
public enum MatchStatus { Scheduled, Live, Final }

public sealed class Fixture
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? GameId { get; set; }
    public Game? Game { get; set; }

    public Guid? TournamentId { get; set; }
    public Tournament? Tournament { get; set; }

    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }

    public Guid? TeamAId { get; set; }
    public Guid? TeamBId { get; set; }

    public int? TeamAScore { get; set; }
    public int? TeamBScore { get; set; }

    public MatchType MatchType { get; set; } = MatchType.Friendly;
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

    public string? CreatedByUserId { get; set; }
    public string? Notes { get; set; }

    public ICollection<FixtureTeam> FixtureTeams { get; set; } = new List<FixtureTeam>();
    public ICollection<FixtureParticipant> Participants { get; set; } = new List<FixtureParticipant>();
}