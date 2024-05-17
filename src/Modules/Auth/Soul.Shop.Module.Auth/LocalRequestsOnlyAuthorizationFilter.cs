using Shop.Module.BasicAuth.Dashboard;
using Soul.Shop.Module.Auth.Dashboard;

namespace Soul.Shop.Module.Auth;

public class LocalRequestsOnlyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // if unknown, assume not local
        if (string.IsNullOrEmpty(context.Request.RemoteIpAddress))
            return false;

        // check if localhost
        if (context.Request.RemoteIpAddress == "127.0.0.1" || context.Request.RemoteIpAddress == "::1")
            return true;

        // compare with local address
        if (context.Request.RemoteIpAddress == context.Request.LocalIpAddress)
            return true;

        return false;
    }
}