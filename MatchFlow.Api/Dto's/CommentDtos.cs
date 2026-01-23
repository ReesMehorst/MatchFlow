using System;

namespace MatchFlow.Api.Dtos;

public record CommentDto(
    Guid Id, 
    Guid PostId, 
    string? AuthorUserId, 
    Guid? AuthorTeamId, 
    string Body, 
    DateTimeOffset CreatedAt, 
    Guid? ParentCommentId
);

public record CreateCommentDto(
    Guid PostId, 
    string? AuthorUserId, 
    Guid? AuthorTeamId, 
    string Body, 
    Guid? ParentCommentId
);

public record UpdateCommentDto(
    string Body
);