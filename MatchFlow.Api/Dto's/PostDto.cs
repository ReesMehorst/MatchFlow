using System;

namespace MatchFlow.Api.Dtos;

public record PostDto(Guid Id, Guid? GameId, Guid? AuthorTeamId, string? AuthorUserId, string? ContentText, DateTimeOffset CreatedAt, int Visibility);
public record CreatePostDto(Guid? GameId, Guid? AuthorTeamId, string? AuthorUserId, string? ContentText, int Visibility);
public record UpdatePostDto(string? ContentText, int Visibility);