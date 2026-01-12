using MatchFlow.Domain.Entities;

namespace MatchFlow.Api.Models;

public sealed class GameCreateDto
{
    public string Name { get; set; } = string.Empty; // unique
    public string? Genre { get; set; }
    public string? IconUrl { get; set; }
}