namespace MatchFlow.Domain.Entities;

public sealed class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // unique
    public string? Genre { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<TeamGame> TeamGames { get; set; } = new List<TeamGame>();
    public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
}