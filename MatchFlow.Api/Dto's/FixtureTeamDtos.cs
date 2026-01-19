using System;

namespace MatchFlow.Api.Dtos;

public record FixtureTeamDto(
    Guid FixtureId, 
    Guid TeamId, 
    int Side, 
    int? Score, 
    string? Result
);

public record CreateFixtureTeamDto(
    Guid FixtureId, 
    Guid TeamId, 
    int Side, 
    int? Score, 
    string? Result
);

public record UpdateFixtureTeamDto(
    int Side, 
    int? Score, 
    string? Result
);