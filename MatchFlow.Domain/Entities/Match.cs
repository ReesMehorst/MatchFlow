namespace MatchFlow.Domain.Entities;

public sealed class GameMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TournamentId { get; set; }
    public Tournament? Tournament { get; set; }

    public Guid? TeamAId { get; set; }
    public Guid? TeamBId { get; set; }

    public int? TeamAScore { get; set; }
    public int? TeamBScore { get; set; }
}
