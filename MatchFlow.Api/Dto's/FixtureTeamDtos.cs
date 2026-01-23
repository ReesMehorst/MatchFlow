using FixtureSide = MatchFlow.Domain.Entities.FixtureSide;
using System;

namespace MatchFlow.Api.Dtos;

public record FixtureTeamDto(
    Guid FixtureId, 
    Guid TeamId, 
    FixtureSide Side, 
    int? Score, 
    string? Result
);

public record CreateFixtureTeamDto(
    Guid FixtureId, 
    Guid TeamId, 
    FixtureSide Side, 
    int? Score, 
    string? Result
);

public record UpdateFixtureTeamDto(
    FixtureSide Side, 
    int? Score, 
    string? Result
);