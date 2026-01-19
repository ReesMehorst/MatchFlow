using System;

namespace MatchFlow.Api.Dtos;

public record TeamGameDto(
    Guid TeamId, 
    Guid GameId, 
    bool Primary
);

public record CreateTeamGameDto(
    Guid TeamId, 
    Guid GameId, 
    bool Primary
);

public record UpdateTeamGameDto(
    bool Primary
);