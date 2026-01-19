using System;

namespace MatchFlow.Api.Dtos;

public record PostLikeDto(
    Guid PostId, 
    string UserId, 
    Guid TeamId
);

public record CreatePostLikeDto(
    Guid PostId, 
    string UserId, 
    Guid TeamId
);