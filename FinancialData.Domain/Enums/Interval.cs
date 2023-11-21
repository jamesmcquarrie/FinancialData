using Ardalis.SmartEnum;

namespace FinancialData.Domain.Enums;

public sealed class Interval : SmartEnum<Interval>
{
    public static readonly Interval OneMinute = new Interval("1min", 1);
    public static readonly Interval FiveMinute = new Interval("5min", 2);
    public static readonly Interval FifteenMinute = new Interval("15min", 3);
    public static readonly Interval ThirtyMinute = new Interval("30min", 4);
    public static readonly Interval OneHour = new Interval("1h", 5);

    private Interval(string name, int value) : base(name, value) { }
}
