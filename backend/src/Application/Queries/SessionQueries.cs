namespace Motocross.Application.Queries;

public record GetSessionQuery(Guid SessionId);

public record GetActiveSessionQuery();

public record GetSessionHistoryQuery(
    int PageNumber = 1,
    int PageSize = 20);

public record GetSessionLapsQuery(Guid SessionId);
