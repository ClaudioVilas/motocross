namespace Motocross.Domain.ValueObjects;

/// <summary>
/// Represents speed in kilometers per hour
/// </summary>
public record Speed
{
    public double KilometersPerHour { get; init; }

    public Speed(double kilometersPerHour)
    {
        if (kilometersPerHour < 0)
            throw new ArgumentException("Speed cannot be negative", nameof(kilometersPerHour));

        if (kilometersPerHour > 500) // Reasonable max for motorsports
            throw new ArgumentException("Speed exceeds maximum reasonable value", nameof(kilometersPerHour));

        KilometersPerHour = kilometersPerHour;
    }

    public double MetersPerSecond => KilometersPerHour / 3.6;
    public double MilesPerHour => KilometersPerHour * 0.621371;

    public static Speed FromMetersPerSecond(double metersPerSecond)
        => new(metersPerSecond * 3.6);

    public static Speed Zero => new(0);

    public override string ToString() => $"{KilometersPerHour:F2} km/h";
}
