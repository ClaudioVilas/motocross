using Motocross.Application.DTOs;

namespace Motocross.Application.Interfaces;

/// <summary>
/// Abstraction for real-time communication publisher
/// </summary>
public interface IRealtimePublisher
{
    Task PublishTrackingUpdateAsync(Guid sessionId, TrackingPointDto trackingPoint);
    Task PublishLapCompletedAsync(Guid sessionId, LapDto lap);
    Task PublishSessionStatusChangedAsync(Guid sessionId, string status);
}
