using MatchFlow.Domain.Entities;
using MatchType = MatchFlow.Domain.Entities.MatchType;
using MatchStatus = MatchFlow.Domain.Entities.MatchStatus;

namespace MatchFlow.Api.Models;

public sealed class FixtureCreateDto
{
    public Guid? GameId { get; set; }
    public Guid? TournamentId { get; set; }
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
}