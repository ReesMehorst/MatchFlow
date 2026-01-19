using System;

namespace MatchFlow.Api.Dtos;

public record GameDto(Guid Id, string Name, string? IconUrl, string? Genre);
public record CreateGameDto(string Name, string? IconUrl, string? Genre);
public record UpdateGameDto(string Name, string? IconUrl, string? Genre);