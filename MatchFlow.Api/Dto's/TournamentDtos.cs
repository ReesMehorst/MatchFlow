using System;

namespace MatchFlow.Api.Dtos;

public record TournamentDto(Guid Id, string Name);
public record CreateTournamentDto(string Name);
public record UpdateTournamentDto(string Name);