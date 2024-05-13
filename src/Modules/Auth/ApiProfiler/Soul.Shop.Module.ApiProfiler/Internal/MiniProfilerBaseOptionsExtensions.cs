namespace Soul.Shop.Module.ApiProfiler.Internal;

public static class MiniProfilerBaseOptionsExtensions
{
    public static List<Guid> ExpireAndGetUnviewed(this MiniProfilerBaseOptions options, string user)
    {
        var ids = options.Storage?.GetUnviewedIds(user);
        if (!(ids?.Count > options.MaxUnviewedProfiles)) return ids;
        for (var i = 0; i < ids.Count - options.MaxUnviewedProfiles; i++)
            options.Storage.SetViewedAsync(user, ids[i]);
        return ids;
    }

    public static async Task<List<Guid>> ExpireAndGetUnviewedAsync(this MiniProfilerBaseOptions options, string user)
    {
        if (options.Storage == null) return null;
        var ids = await options.Storage.GetUnviewedIdsAsync(user).ConfigureAwait(false);
        if (!(ids?.Count > options.MaxUnviewedProfiles)) return ids;
        for (var i = 0; i < ids.Count - options.MaxUnviewedProfiles; i++)
            await options.Storage.SetViewedAsync(user, ids[i]).ConfigureAwait(false);
        return ids;
    }
}