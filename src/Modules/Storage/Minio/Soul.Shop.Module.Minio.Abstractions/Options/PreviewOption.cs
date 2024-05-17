namespace Soul.Shop.Module.Minio.Abstractions.Options;

public class PreviewOption
{
    public const string Position = "Preview";
    public bool Enabled { get; set; }
    public string[] FileTypes { get; set; }
    public TimeExpire? TimeExpire { get; set; }
}

public class TimeExpire
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
