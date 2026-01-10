namespace MatchFlow.Domain.Entities;

public interface IUser
{
    string Id { get; }
    string UserName { get; }
    string DisplayName { get; set; }
    string? Bio { get; set; }
    string? AvatarUrl { get; set; }
    DateTimeOffset CreatedAt { get; set; }
}