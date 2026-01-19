using Visibility = MatchFlow.Domain.Entities.Visibility;
using System;

namespace MatchFlow.Api.Dtos;

public record PostDto(
    Guid Id,
    Guid? GameId, 
    Guid? AuthorTeamId, 
    string? AuthorUserId, 
    string? ContentText, 
    DateTimeOffset CreatedAt, 
    Visibility Visibility
);

public record CreatePostDto(
    Guid? GameId,
    Guid? AuthorTeamId, 
    string? AuthorUserId, 
    string? ContentText, 
    Visibility Visibility
);

public record UpdatePostDto(
    string? ContentText, 
    Visibility Visibility
);