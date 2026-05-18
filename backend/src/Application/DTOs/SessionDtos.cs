namespace Motocross.Application.DTOs;

public record CoordinateDto(
    double Latitude,
    double Longitude);

public record SpeedDto(
    double KilometersPerHour,
    double MetersPerSecond,
    double MilesPerHour);

public record TrackingPointDto(
    Guid Id,
    Guid SessionId,
    CoordinateDto Coordinate,
    SpeedDto Speed,
    DateTime Timestamp,
    string Type,
    double? Altitude,
    double? Accuracy,
    double? Heading);

public record LapDto(
    Guid Id,
    Guid SessionId,
    int LapNumber,
    DateTime StartTime,
    DateTime EndTime,
    string Duration,
    double DurationSeconds,
    SpeedDto AverageSpeed,
    SpeedDto MaxSpeed,
    double DistanceMeters);

public record SessionDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime StartTime,
    DateTime? EndTime,
    string Status,
    CoordinateDto? StartFinishLine,
    double StartFinishLineRadius,
    int TotalLaps,
    LapDto? BestLap,
    string? TotalDuration,
    double TotalDistanceMeters,
    List<LapDto> Laps);

public record SessionSummaryDto(
    Guid Id,
    string Name,
    DateTime StartTime,
    DateTime? EndTime,
    string Status,
    int TotalLaps);
