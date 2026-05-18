using Motocross.Application.Commands;
using Motocross.Application.DTOs;
using Motocross.Application.Interfaces;
using Motocross.Application.Queries;
using Motocross.Domain.Abstractions;
using Motocross.Domain.Entities;
using Motocross.Domain.Enums;
using Motocross.Domain.ValueObjects;

namespace Motocross.Application.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;
    private readonly ILapDetectionService _lapDetectionService;
    private readonly IRealtimePublisher _realtimePublisher;

    public SessionService(
        ISessionRepository repository,
        ILapDetectionService lapDetectionService,
        IRealtimePublisher realtimePublisher)
    {
        _repository = repository;
        _lapDetectionService = lapDetectionService;
        _realtimePublisher = realtimePublisher;
    }

    public async Task<SessionDto> CreateSessionAsync(CreateSessionCommand command)
    {
        var session = new Session(command.Name, command.Description);
        await _repository.AddAsync(session);

        return MapToDto(session);
    }

    public async Task<SessionDto> StartSessionAsync(StartSessionCommand command)
    {
        var session = await _repository.GetByIdAsync(command.SessionId)
            ?? throw new InvalidOperationException($"Session {command.SessionId} not found");

        session.Start();
        await _repository.UpdateAsync(session);

        await _realtimePublisher.PublishSessionStatusChangedAsync(session.Id, session.Status.ToString());

        return MapToDto(session);
    }

    public async Task<SessionDto> CompleteSessionAsync(CompleteSessionCommand command)
    {
        var session = await _repository.GetByIdAsync(command.SessionId)
            ?? throw new InvalidOperationException($"Session {command.SessionId} not found");

        session.Complete();
        await _repository.UpdateAsync(session);

        await _realtimePublisher.PublishSessionStatusChangedAsync(session.Id, session.Status.ToString());

        return MapToDto(session);
    }

    public async Task SetStartFinishLineAsync(SetStartFinishLineCommand command)
    {
        var session = await _repository.GetByIdAsync(command.SessionId)
            ?? throw new InvalidOperationException($"Session {command.SessionId} not found");

        var coordinate = new Coordinate(command.Latitude, command.Longitude);
        session.SetStartFinishLine(coordinate, command.RadiusMeters);

        await _repository.UpdateAsync(session);
    }

    public async Task RecordTrackingPointAsync(RecordTrackingPointCommand command)
    {
        var session = await _repository.GetByIdAsync(command.SessionId)
            ?? throw new InvalidOperationException($"Session {command.SessionId} not found");

        var coordinate = new Coordinate(command.Latitude, command.Longitude);
        var speed = new Speed(command.SpeedKmh);

        var trackingPoint = new TrackingPoint(
            coordinate,
            speed,
            command.Timestamp,
            TrackingPointType.Normal,
            command.Altitude,
            command.Accuracy,
            command.Heading
        );

        session.AddTrackingPoint(trackingPoint);

        // Check for lap completion
        var lap = _lapDetectionService.DetectLapCompletion(session);
        if (lap != null)
        {
            session.AddLap(lap);
            await _repository.UpdateAsync(session);

            // Publish lap completion
            var lapDto = MapLapToDto(lap);
            await _realtimePublisher.PublishLapCompletedAsync(session.Id, lapDto);
        }
        else
        {
            await _repository.UpdateAsync(session);
        }

        // Publish tracking update
        var trackingPointDto = MapTrackingPointToDto(trackingPoint);
        await _realtimePublisher.PublishTrackingUpdateAsync(session.Id, trackingPointDto);
    }

    public async Task<SessionDto?> GetSessionAsync(GetSessionQuery query)
    {
        var session = await _repository.GetByIdAsync(query.SessionId);
        return session != null ? MapToDto(session) : null;
    }

    public async Task<SessionDto?> GetActiveSessionAsync()
    {
        var session = await _repository.GetActiveSessionAsync();
        return session != null ? MapToDto(session) : null;
    }

    public async Task<List<SessionSummaryDto>> GetSessionHistoryAsync(GetSessionHistoryQuery query)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;
        var sessions = await _repository.GetAllAsync(skip, query.PageSize);

        return sessions.Select(MapToSummaryDto).ToList();
    }

    public async Task<List<LapDto>> GetSessionLapsAsync(GetSessionLapsQuery query)
    {
        var session = await _repository.GetByIdAsync(query.SessionId);
        return session?.Laps.Select(MapLapToDto).ToList() ?? new List<LapDto>();
    }

    // Mapping methods
    private SessionDto MapToDto(Session session)
    {
        return new SessionDto(
            session.Id,
            session.Name,
            session.Description,
            session.StartTime,
            session.EndTime,
            session.Status.ToString(),
            session.StartFinishLine != null
                ? new CoordinateDto(session.StartFinishLine.Latitude, session.StartFinishLine.Longitude)
                : null,
            session.StartFinishLineRadius,
            session.TotalLaps,
            session.BestLap != null ? MapLapToDto(session.BestLap) : null,
            session.TotalDuration?.ToString(),
            session.GetTotalDistanceMeters(),
            session.Laps.Select(MapLapToDto).ToList()
        );
    }

    private SessionSummaryDto MapToSummaryDto(Session session)
    {
        return new SessionSummaryDto(
            session.Id,
            session.Name,
            session.StartTime,
            session.EndTime,
            session.Status.ToString(),
            session.TotalLaps
        );
    }

    private LapDto MapLapToDto(Lap lap)
    {
        return new LapDto(
            lap.Id,
            lap.SessionId,
            lap.LapNumber,
            lap.StartTime,
            lap.EndTime,
            lap.Duration.ToString(),
            lap.Duration.TotalSeconds,
            MapSpeedToDto(lap.AverageSpeed),
            MapSpeedToDto(lap.MaxSpeed),
            lap.DistanceMeters
        );
    }

    private TrackingPointDto MapTrackingPointToDto(TrackingPoint point)
    {
        return new TrackingPointDto(
            point.Id,
            point.SessionId,
            new CoordinateDto(point.Coordinate.Latitude, point.Coordinate.Longitude),
            MapSpeedToDto(point.Speed),
            point.Timestamp,
            point.Type.ToString(),
            point.Altitude,
            point.Accuracy,
            point.Heading
        );
    }

    private SpeedDto MapSpeedToDto(Speed speed)
    {
        return new SpeedDto(
            speed.KilometersPerHour,
            speed.MetersPerSecond,
            speed.MilesPerHour
        );
    }
}
