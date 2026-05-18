using Motocross.Application.Commands;
using Motocross.Application.DTOs;
using Motocross.Application.Queries;

namespace Motocross.Application.Interfaces;

public interface ISessionService
{
    // Commands
    Task<SessionDto> CreateSessionAsync(CreateSessionCommand command);
    Task<SessionDto> StartSessionAsync(StartSessionCommand command);
    Task<SessionDto> CompleteSessionAsync(CompleteSessionCommand command);
    Task SetStartFinishLineAsync(SetStartFinishLineCommand command);
    Task RecordTrackingPointAsync(RecordTrackingPointCommand command);

    // Queries
    Task<SessionDto?> GetSessionAsync(GetSessionQuery query);
    Task<SessionDto?> GetActiveSessionAsync();
    Task<List<SessionSummaryDto>> GetSessionHistoryAsync(GetSessionHistoryQuery query);
    Task<List<LapDto>> GetSessionLapsAsync(GetSessionLapsQuery query);
}
