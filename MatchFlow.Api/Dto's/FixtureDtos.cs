using System;
using MatchType = MatchFlow.Domain.Entities.MatchType;
using MatchStatus = MatchFlow.Domain.Entities.MatchStatus;

namespace MatchFlow.Api.Dtos;

public record FixtureDto(
    Guid Id,
    Guid? TournamentId,
    Guid? GameId, 
    Guid? TeamAId, 
    Guid? TeamBId, 
    int? TeamAScore, 
    int? TeamBScore, 
    DateTimeOffset? StartAt, 
    DateTimeOffset? EndAt, 
    MatchStatus Status, 
    MatchType MatchType, 
    string? Notes, 
    string? CreatedByUserId
);

public record CreateFixtureDto(
    Guid? TournamentId, 
    Guid? GameId, 
    Guid? TeamAId, 
    Guid? TeamBId, 
    int? TeamAScore, 
    int? TeamBScore, 
    DateTimeOffset? StartAt, 
    DateTimeOffset? EndAt, 
    MatchStatus Status, 
    MatchType MatchType, 
    string? Notes, 
    string? CreatedByUserId
);

public record UpdateFixtureDto(
    Guid? TournamentId, 
    Guid? GameId, 
    Guid? TeamAId, 
    Guid? TeamBId, 
    int? TeamAScore, 
    int? TeamBScore, 
    DateTimeOffset? StartAt, 
    DateTimeOffset? EndAt, 
    MatchStatus Status, 
    MatchType MatchType, 
    string? Notes
);