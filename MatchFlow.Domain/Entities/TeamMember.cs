namespace MatchFlow.Domain.Entities;

public sealed class TeamMember
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Member"; // Possible roles: Member, Captain, Coach, IGL, Fragger, Owner etc.

    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LeftAt { get; set; } // Null if still a member
}
