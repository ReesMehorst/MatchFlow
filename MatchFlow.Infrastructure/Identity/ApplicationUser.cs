using Microsoft.AspNetCore.Identity;
using MatchFlow.Domain.Entities;

namespace MatchFlow.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser, IUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Explicitly implement IUser.UserName to match non-nullable contract
    string IUser.UserName => UserName ?? string.Empty;
}