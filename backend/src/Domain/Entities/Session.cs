using Motocross.Domain.Enums;
using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Entities;

/// <summary>
/// Represents a tracking session (practice, race, etc.)
/// </summary>
public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public SessionStatus Status { get; private set; }
    public Coordinate? StartFinishLine { get; private set; }
    public double StartFinishLineRadius { get; private set; } = 20; // meters
    public Guid? UserId { get; private set; }
    public UserAccount? User { get; private set; }

    private readonly List<TrackingPoint> _trackingPoints = new();
    public IReadOnlyCollection<TrackingPoint> TrackingPoints => _trackingPoints.AsReadOnly();

    private readonly List<Lap> _laps = new();
    public IReadOnlyCollection<Lap> Laps => _laps.AsReadOnly();

    public int TotalLaps => _laps.Count;
    public Lap? BestLap => _laps.OrderBy(l => l.Duration.TotalSeconds).FirstOrDefault();
    public Duration? TotalDuration => EndTime.HasValue 
        ? Duration.Between(StartTime, EndTime.Value) 
        : null;

    private Session() { } // EF Core

    public Session(string name, string? description = null, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Session name is required", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        StartTime = DateTime.UtcNow;
        Status = SessionStatus.Created;
        UserId = userId;
    }

    public void Start()
    {
        if (Status != SessionStatus.Created)
            throw new InvalidOperationException("Can only start a created session");

        Status = SessionStatus.Active;
        StartTime = DateTime.UtcNow;
    }

    public void Pause()
    {
        if (Status != SessionStatus.Active)
            throw new InvalidOperationException("Can only pause an active session");

        Status = SessionStatus.Paused;
    }

    public void Resume()
    {
        if (Status != SessionStatus.Paused)
            throw new InvalidOperationException("Can only resume a paused session");

        Status = SessionStatus.Active;
    }

    public void Complete()
    {
        if (Status == SessionStatus.Completed || Status == SessionStatus.Cancelled)
            throw new InvalidOperationException("Session is already finished");

        EndTime = DateTime.UtcNow;
        Status = SessionStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == SessionStatus.Completed || Status == SessionStatus.Cancelled)
            throw new InvalidOperationException("Session is already finished");

        EndTime = DateTime.UtcNow;
        Status = SessionStatus.Cancelled;
    }

    public void SetStartFinishLine(Coordinate coordinate, double radiusMeters = 20)
    {
        StartFinishLine = coordinate ?? throw new ArgumentNullException(nameof(coordinate));
        StartFinishLineRadius = radiusMeters > 0 ? radiusMeters : throw new ArgumentException("Radius must be positive");
    }

    public void AddTrackingPoint(TrackingPoint point)
    {
        if (Status != SessionStatus.Active)
            throw new InvalidOperationException("Cannot add tracking points to inactive session");

        _trackingPoints.Add(point ?? throw new ArgumentNullException(nameof(point)));
    }

    public void AddLap(Lap lap)
    {
        if (lap == null)
            throw new ArgumentNullException(nameof(lap));

        _laps.Add(lap);
    }

    public bool IsNearStartFinishLine(Coordinate coordinate)
    {
        if (StartFinishLine == null)
            return false;

        var distance = StartFinishLine.DistanceTo(coordinate);
        return distance <= StartFinishLineRadius;
    }

    public double GetTotalDistanceMeters()
    {
        var points = _trackingPoints.OrderBy(p => p.Timestamp).ToList();
        double totalDistance = 0;

        for (int i = 1; i < points.Count; i++)
        {
            totalDistance += points[i - 1].DistanceTo(points[i]);
        }

        return totalDistance;
    }

    public void AssignUser(Guid userId)
    {
        UserId = userId;
    }
}
