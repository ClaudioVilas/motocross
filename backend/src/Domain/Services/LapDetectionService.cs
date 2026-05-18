using Motocross.Domain.Abstractions;
using Motocross.Domain.Entities;
using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Services;

/// <summary>
/// Domain service for detecting lap completions based on start/finish line crossing
/// </summary>
public class LapDetectionService : ILapDetectionService
{
    public Lap? DetectLapCompletion(Session session)
    {
        if (session.StartFinishLine == null)
            return null;

        var points = session.TrackingPoints
            .OrderBy(p => p.Timestamp)
            .ToList();

        if (points.Count < 10) // Need minimum points to detect a lap
            return null;

        // Get last lap end time (or session start)
        var lastLapEndTime = session.Laps.Any()
            ? session.Laps.OrderBy(l => l.EndTime).Last().EndTime
            : session.StartTime;

        // Get points since last lap
        var pointsSinceLastLap = points
            .Where(p => p.Timestamp > lastLapEndTime)
            .ToList();

        if (pointsSinceLastLap.Count < 10)
            return null;

        // Find start/finish line crossings
        TrackingPoint? lapStartPoint = null;
        TrackingPoint? lapEndPoint = null;

        for (int i = 1; i < pointsSinceLastLap.Count; i++)
        {
            var previous = pointsSinceLastLap[i - 1];
            var current = pointsSinceLastLap[i];

            if (IsCrossingStartFinishLine(session, current, previous))
            {
                if (lapStartPoint == null)
                {
                    lapStartPoint = current;
                }
                else if (lapEndPoint == null)
                {
                    // Check if enough distance covered (at least 100m)
                    var distanceFromStart = lapStartPoint.DistanceTo(current);
                    if (distanceFromStart > 100)
                    {
                        lapEndPoint = current;
                        break;
                    }
                }
            }
        }

        if (lapStartPoint == null || lapEndPoint == null)
            return null;

        // Calculate lap statistics
        var lapPoints = pointsSinceLastLap
            .Where(p => p.Timestamp >= lapStartPoint.Timestamp && p.Timestamp <= lapEndPoint.Timestamp)
            .ToList();

        var maxSpeed = lapPoints.Max(p => p.Speed);
        var avgSpeedKmh = lapPoints.Average(p => p.Speed.KilometersPerHour);
        var avgSpeed = new Speed(avgSpeedKmh);

        // Calculate distance
        double totalDistance = 0;
        for (int i = 1; i < lapPoints.Count; i++)
        {
            totalDistance += lapPoints[i - 1].DistanceTo(lapPoints[i]);
        }

        var lapNumber = session.TotalLaps + 1;

        return new Lap(
            lapNumber,
            lapStartPoint.Timestamp,
            lapEndPoint.Timestamp,
            avgSpeed,
            maxSpeed,
            totalDistance
        );
    }

    public bool IsCrossingStartFinishLine(Session session, TrackingPoint currentPoint, TrackingPoint? previousPoint)
    {
        if (session.StartFinishLine == null || previousPoint == null)
            return false;

        var currentDistance = session.StartFinishLine.DistanceTo(currentPoint.Coordinate);
        var previousDistance = session.StartFinishLine.DistanceTo(previousPoint.Coordinate);

        // Crossing detected if previous was outside radius and current is inside
        // Or vice versa (entering or exiting the radius)
        var currentInside = currentDistance <= session.StartFinishLineRadius;
        var previousInside = previousDistance <= session.StartFinishLineRadius;

        return currentInside && !previousInside;
    }
}
