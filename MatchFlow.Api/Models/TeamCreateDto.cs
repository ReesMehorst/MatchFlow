namespace MatchFlow.Api.Models;

public sealed class TeamCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? LogoUrl { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
}