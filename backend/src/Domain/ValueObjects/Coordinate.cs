namespace Motocross.Domain.ValueObjects;

/// <summary>
/// Represents a geographic coordinate with latitude and longitude
/// </summary>
public record Coordinate
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public Coordinate(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Calculates the distance in meters between this coordinate and another using Haversine formula
    /// </summary>
    public double DistanceTo(Coordinate other)
    {
        const double earthRadiusKm = 6371;

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(Latitude)) *
                Math.Cos(DegreesToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distanceKm = earthRadiusKm * c;

        return distanceKm * 1000; // Convert to meters
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public override string ToString() => $"({Latitude:F6}, {Longitude:F6})";
}
