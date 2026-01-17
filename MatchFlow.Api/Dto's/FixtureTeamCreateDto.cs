using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Models;

public sealed class FixtureTeamCreateDto
{
    public Guid FixtureId { get; set; }

    public Guid TeamId { get; set; }
    public FixtureSide Side { get; set; }

    public int? Score { get; set; }
    public string? Result { get; set; } // Win/Loss/Draw
}