namespace MatchFlow.Api.Dtos;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total
);

public sealed record TeamListItemDto(
    Guid Id,
    string Name,
    string Tag,
    string OwnerUserId,
    string? LogoUrl,
    string? Bio,
    int MemberCount,
    bool IsMember
);