using Motocross.Domain.Enums;
using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Entities;

/// <summary>
/// Represents a single GPS tracking point
/// </summary>
public class TrackingPoint
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Coordinate Coordinate { get; private set; }
    public Speed Speed { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TrackingPointType Type { get; private set; }
    public double? Altitude { get; private set; }
    public double? Accuracy { get; private set; }
    public double? Heading { get; private set; }

    // Navigation property
    public Session Session { get; private set; } = null!;

    private TrackingPoint() { } // EF Core

    public TrackingPoint(
        Coordinate coordinate,
        Speed speed,
        DateTime timestamp,
        TrackingPointType type = TrackingPointType.Normal,
        double? altitude = null,
        double? accuracy = null,
        double? heading = null)
    {
        Id = Guid.NewGuid();
        Coordinate = coordinate ?? throw new ArgumentNullException(nameof(coordinate));
        Speed = speed ?? throw new ArgumentNullException(nameof(speed));
        Timestamp = timestamp;
        Type = type;
        Altitude = altitude;
        Accuracy = accuracy;
        Heading = heading;
    }

    public void MarkAsLapStart() => Type = TrackingPointType.LapStart;
    public void MarkAsLapFinish() => Type = TrackingPointType.LapFinish;

    public double DistanceTo(TrackingPoint other)
        => Coordinate.DistanceTo(other.Coordinate);
}
