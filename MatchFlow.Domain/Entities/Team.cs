using System.Text.Json.Serialization;

namespace MatchFlow.Domain.Entities;

public sealed class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? LogoUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string OwnerUserId { get; set; } = string.Empty;
    [JsonIgnore] public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    [JsonIgnore] public ICollection<TeamGame> TeamGames { get; set; } = new List<TeamGame>();
}
