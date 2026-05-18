using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Abstractions;

/// <summary>
/// Simple abstraction for getting current position
/// </summary>
public interface IPositionSource
{
    /// <summary>
    /// Get the current position
    /// </summary>
    Task<Coordinate> GetCurrentPositionAsync();

    /// <summary>
    /// Get the current position with additional data
    /// </summary>
    Task<PositionUpdate> GetCurrentPositionDetailsAsync();
}
