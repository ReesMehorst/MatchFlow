using System.ComponentModel.DataAnnotations;
using System;

namespace MatchFlow.Api.Dtos;

public record TeamDto(
    Guid Id, 
    string Name, 
    string Tag, 
    string OwnerUserId, 
    string? LogoUrl, 
    string? Bio, 
    DateTimeOffset CreatedAt
);

public record CreateTeamDto(
    [Required, StringLength(100, MinimumLength = 2)] string Name, 
    [Required, StringLength(5, MinimumLength = 2)] string Tag, 
    string OwnerUserId, 
    string? LogoUrl, 
    string? Bio
);

public record UpdateTeamDto(
    string Name, 
    string Tag, 
    string? LogoUrl, 
    string? Bio
);