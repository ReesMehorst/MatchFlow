using System;

namespace MatchFlow.Api.Dtos;

public record FixtureParticipantDto(
    Guid FixtureId, 
    string UserId, 
    Guid TeamId, 
    string? InGameName, 
    string? Role, 
    string? StatsJson
);

public record CreateFixtureParticipantDto(
    Guid FixtureId, 
    string UserId, 
    Guid TeamId, 
    string? InGameName, 
    string? Role, 
    string? StatsJson
);

public record UpdateFixtureParticipantDto(
    Guid TeamId, 
    string? InGameName, 
    string? Role, 
    string? StatsJson
);