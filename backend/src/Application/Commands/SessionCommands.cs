namespace Motocross.Application.Commands;

public record CreateSessionCommand(
    string Name,
    string? Description = null);

public record StartSessionCommand(
    Guid SessionId);

public record CompleteSessionCommand(
    Guid SessionId);

public record SetStartFinishLineCommand(
    Guid SessionId,
    double Latitude,
    double Longitude,
    double RadiusMeters = 20);

public record RecordTrackingPointCommand(
    Guid SessionId,
    double Latitude,
    double Longitude,
    double SpeedKmh,
    DateTime Timestamp,
    double? Altitude = null,
    double? Accuracy = null,
    double? Heading = null);
