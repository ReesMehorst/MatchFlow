namespace MatchFlow.Api.Dtos;

public sealed record RegisterDto(
    string Email, 
    string Password, 
    string DisplayName
);

public sealed record LoginDto(
    string Email, 
    string Password
);

public sealed record AuthResultDto(
    string Token,
    string Email,
    string DisplayName,
    string[] Roles
);

public sealed record MeDto(
    string Id,
    string Email,
    string? DisplayName,
    string[] Roles
);