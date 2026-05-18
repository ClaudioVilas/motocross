using Motocross.Domain.ValueObjects;

namespace Motocross.Domain.Abstractions;

/// <summary>
/// Represents a position update from any tracking source
/// </summary>
public record PositionUpdate(
    Coordinate Coordinate,
    Speed Speed,
    DateTime Timestamp,
    double? Altitude = null,
    double? Accuracy = null,
    double? Heading = null);

/// <summary>
/// Abstraction for any tracking provider (Mobile GPS, BLE, External GPS, etc.)
/// </summary>
public interface ITrackingProvider
{
    /// <summary>
    /// Unique name identifier for this provider
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Check if this provider is available and operational
    /// </summary>
    Task<bool> IsAvailableAsync();

    /// <summary>
    /// Stream position updates asynchronously
    /// </summary>
    IAsyncEnumerable<PositionUpdate> StreamPositionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initialize the provider with configuration
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Clean up resources
    /// </summary>
    Task DisposeAsync();
}
