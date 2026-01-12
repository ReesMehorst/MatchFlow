using System.Text.RegularExpressions;

namespace MatchFlow.Domain.Entities;

public sealed class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<Fixture> Fixtures { get; set; } = [];
}