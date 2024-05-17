namespace Soul.Shop.Module.Minio.Abstractions.Options;

public class SettingCornJob
{
    public bool Enable { get; set; } = true;
    public TimeScanDelay? TimeScanDelay { get; set; }
    public List<string> BucketScanner { get; set; }
    public TimeBonusExpire? TimeBonusExpire { get; set; }

    public bool IsEnable()
    {
        return Enable;
    }
}

public class TimeScanDelay
{
    public TypeTime TypeTime { get; set; }
    public int Value { get; set; }

    public int GetTime()
    {
        return TypeTime switch
        {
            TypeTime.Second => Value,
            TypeTime.Minute => Value * 60,
            TypeTime.Hour => Value * 60 * 60,
            TypeTime.Day => Value * 60 * 60 * 24,
            TypeTime.Week => Value * 60 * 60 * 24 * 7,
            TypeTime.Month => Value * 60 * 60 * 24 * 30,
            TypeTime.Year => Value * 60 * 60 * 24 * 365,
            _ => 0
        };
    }
}

public class TimeBonusExpire
{
    public TypeTime TypeTime { get; set; }
    public int Value { get; set; }

    public int GetTime()
    {
        return TypeTime switch
        {
            TypeTime.Second => Value,
            TypeTime.Minute => Value * 60,
            TypeTime.Hour => Value * 60 * 60,
            TypeTime.Day => Value * 60 * 60 * 24,
            TypeTime.Week => Value * 60 * 60 * 24 * 7,
            TypeTime.Month => Value * 60 * 60 * 24 * 30,
            TypeTime.Year => Value * 60 * 60 * 24 * 365,
            _ => 0
        };
    }
}

public enum TypeTime : int
{
    Second = 1,
    Minute = 2,
    Hour = 3,
    Day = 4,
    Week = 5,
    Month = 6,
    Year = 7
}
