using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Models;

public sealed class FixtureParticipantCreateDto
{
    public Guid FixtureId { get; set; }
    public Guid TeamId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? InGameName { get; set; }
    public string? Role { get; set; }
    public string? StatsJson { get; set; }
}