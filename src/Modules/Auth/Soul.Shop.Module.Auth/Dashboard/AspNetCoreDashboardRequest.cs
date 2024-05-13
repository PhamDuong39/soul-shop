using Microsoft.AspNetCore.Http;
using Shop.Module.BasicAuth.Dashboard;

namespace Soul.Shop.Module.Auth.Dashboard;

internal sealed class AspNetCoreDashboardRequest(HttpContext context) : DashboardRequest
{
    private readonly HttpContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public override string Method => _context.Request.Method;
    public override string Path => _context.Request.Path.Value;
    public override string PathBase => _context.Request.PathBase.Value;
    public override string LocalIpAddress => _context.Connection.LocalIpAddress.ToString();
    public override string RemoteIpAddress => _context.Connection.RemoteIpAddress.ToString();

    public override string GetQuery(string key)
    {
        return _context.Request.Query[key];
    }

    public override async Task<IList<string>> GetFormValuesAsync(string key)
    {
        var form = await _context.Request.ReadFormAsync();
        return form[key];
    }
}