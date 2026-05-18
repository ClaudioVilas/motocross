using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Entities;

/// <summary>
/// Represents a completed lap in a session
/// </summary>
public class Lap
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public int LapNumber { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public Duration Duration { get; private set; }
    public Speed AverageSpeed { get; private set; }
    public Speed MaxSpeed { get; private set; }
    public double DistanceMeters { get; private set; }

    // Navigation property
    public Session Session { get; private set; } = null!;

    private Lap() { } // EF Core

    public Lap(
        int lapNumber,
        DateTime startTime,
        DateTime endTime,
        Speed averageSpeed,
        Speed maxSpeed,
        double distanceMeters)
    {
        if (lapNumber <= 0)
            throw new ArgumentException("Lap number must be positive", nameof(lapNumber));

        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time", nameof(endTime));

        Id = Guid.NewGuid();
        LapNumber = lapNumber;
        StartTime = startTime;
        EndTime = endTime;
        Duration = Duration.Between(startTime, endTime);
        AverageSpeed = averageSpeed ?? throw new ArgumentNullException(nameof(averageSpeed));
        MaxSpeed = maxSpeed ?? throw new ArgumentNullException(nameof(maxSpeed));
        DistanceMeters = distanceMeters;
    }

    public bool IsFasterThan(Lap other)
        => Duration.TotalSeconds < other.Duration.TotalSeconds;
}
