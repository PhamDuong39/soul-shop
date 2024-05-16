using System.Globalization;
using Microsoft.AspNetCore.Http;
using Shop.Module.BasicAuth.Dashboard;

namespace Soul.Shop.Module.Auth.Dashboard;

internal sealed class AspNetCoreDashboardResponse(HttpContext context) : DashboardResponse
{
    private readonly HttpContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public override string ContentType
    {
        get => _context.Response.ContentType;
        set => _context.Response.ContentType = value;
    }

    public override int StatusCode
    {
        get => _context.Response.StatusCode;
        set => _context.Response.StatusCode = value;
    }

    public override Stream Body => _context.Response.Body;

    public override Task WriteAsync(string text)
    {
        return _context.Response.WriteAsync(text);
    }

    public override void SetExpire(DateTimeOffset? value)
    {
        _context.Response.Headers["Expires"] = value?.ToString("r", CultureInfo.InvariantCulture);
    }
}