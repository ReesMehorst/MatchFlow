using System;

namespace MatchFlow.Api.Dtos;

public record UserGameDto(string UserId, Guid GameId, int? RankElo);
public record CreateUserGameDto(string UserId, Guid GameId, int? RankElo);
public record UpdateUserGameDto(int? RankElo);