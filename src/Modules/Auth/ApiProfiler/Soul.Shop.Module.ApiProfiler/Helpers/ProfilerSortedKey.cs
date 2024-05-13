namespace Soul.Shop.Module.ApiProfiler.Helpers;

public class ProfilerSortedKey(MiniProfiler profiler) : IComparable<ProfilerSortedKey>
{
    public Guid Id { get; } = profiler.Id;

    public DateTime Started { get; } = profiler.Started;


    public int CompareTo(ProfilerSortedKey other)
    {
        var comp = Started.CompareTo(other.Started);
        if (comp == 0) comp = Id.CompareTo(other.Id);
        return comp;
    }
}

public static class ProfilerSortedKeyExtensions
{
    public static int BinaryClosestSearch<T>(this SortedList<ProfilerSortedKey, T> list, DateTime date)
    {
        var lower = 0;
        var upper = list.Count - 1;

        while (lower <= upper)
        {
            var adjustedIndex = lower + ((upper - lower) >> 1);
            var comparison = list.Keys[adjustedIndex].Started.CompareTo(date);
            switch (comparison)
            {
                case 0:
                    return adjustedIndex;
                case < 0:
                    lower = adjustedIndex + 1;
                    break;
                default:
                    upper = adjustedIndex - 1;
                    break;
            }
        }

        return lower;
    }
}