namespace Motocross.Domain.ValueObjects;

/// <summary>
/// Represents a time duration for laps and sessions
/// </summary>
public record Duration
{
    public TimeSpan Value { get; init; }

    public Duration(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            throw new ArgumentException("Duration cannot be negative", nameof(value));

        Value = value;
    }

    public double TotalSeconds => Value.TotalSeconds;
    public double TotalMinutes => Value.TotalMinutes;

    public static Duration FromSeconds(double seconds)
        => new(TimeSpan.FromSeconds(seconds));

    public static Duration Between(DateTime start, DateTime end)
        => new(end - start);

    public override string ToString()
    {
        if (Value.TotalHours >= 1)
            return $"{Value.Hours:D2}:{Value.Minutes:D2}:{Value.Seconds:D2}.{Value.Milliseconds:D3}";

        return $"{Value.Minutes:D2}:{Value.Seconds:D2}.{Value.Milliseconds:D3}";
    }
}
