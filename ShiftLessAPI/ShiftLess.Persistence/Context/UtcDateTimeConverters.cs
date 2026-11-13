using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ShiftLess.Persistence.Context;

/// <summary>
/// Postgres (timestamp with time zone) only accepts DateTime values with
/// Kind=Utc. This converter normalizes every DateTime on the way in and
/// out: Local is converted, Unspecified is treated as already-UTC.
/// </summary>
public sealed class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => ToUtc(v),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }

    public static DateTime ToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}

public sealed class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter()
        : base(
            v => v.HasValue ? UtcDateTimeConverter.ToUtc(v.Value) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    {
    }
}