using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Shop.Module.BasicAuth.Dashboard;
using Soul.Shop.Module.Auth.Dashboard;

namespace Soul.Shop.Module.Auth;

public class BasicAuthAuthorizationFilter(BasicAuthAuthorizationFilterOptions options) : IDashboardAuthorizationFilter
{
    public BasicAuthAuthorizationFilter()
        : this(new BasicAuthAuthorizationFilterOptions())
    {
    }

    private bool Challenge(HttpContext context)
    {
        context.Response.StatusCode = 401;
        context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"BaseAuth Dashboard\"");
        return false;
    }

    public bool Authorize(DashboardContext _context)
    {
        var context = _context.GetHttpContext();
        if (options.SslRedirect == true && context.Request.Scheme != "https")
        {
            var redirectUri = new UriBuilder("https", context.Request.Host.ToString(), 443, context.Request.Path)
                .ToString();

            context.Response.StatusCode = 301;
            context.Response.Redirect(redirectUri);
            return false;
        }

        if (options.RequireSsl == true && context.Request.IsHttps == false) return false;

        string header = context.Request.Headers["Authorization"];

        if (string.IsNullOrWhiteSpace(header) == false)
        {
            var authValues = AuthenticationHeaderValue.Parse(header);

            if ("Basic".Equals(authValues.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
                var parts = parameter.Split(':');

                if (parts.Length > 1)
                {
                    var login = parts[0];
                    var password = parts[1];

                    if (string.IsNullOrWhiteSpace(login) == false && string.IsNullOrWhiteSpace(password) == false)
                        return options
                                   .Users
                                   .Any(user => user.Validate(login, password, options.LoginCaseSensitive))
                               || Challenge(context);
                }
            }
        }

        return Challenge(context);
    }
}