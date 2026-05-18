using Motocross.Domain.Entities;

namespace Motocross.Domain.Abstractions;

/// <summary>
/// Domain service for detecting lap completion
/// </summary>
public interface ILapDetectionService
{
    /// <summary>
    /// Analyze tracking points to detect if a lap has been completed
    /// </summary>
    /// <returns>A new Lap if detected, null otherwise</returns>
    Lap? DetectLapCompletion(Session session);

    /// <summary>
    /// Check if a specific tracking point crossed the start/finish line
    /// </summary>
    bool IsCrossingStartFinishLine(Session session, TrackingPoint currentPoint, TrackingPoint? previousPoint);
}
