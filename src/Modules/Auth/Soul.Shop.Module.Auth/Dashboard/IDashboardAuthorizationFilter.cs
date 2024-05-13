using Soul.Shop.Module.Auth.Dashboard;

namespace Shop.Module.BasicAuth.Dashboard;

public interface IDashboardAuthorizationFilter
{
    bool Authorize(DashboardContext context);
}