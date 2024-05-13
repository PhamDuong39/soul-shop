using Shop.Module.BasicAuth.Dashboard;

namespace Soul.Shop.Module.Auth.Dashboard;

public class DashboardOptions
{
    public DashboardOptions()
    {
        Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() };
    }

    public IEnumerable<IDashboardAuthorizationFilter> Authorization { get; set; }

    public bool IgnoreAntiforgeryToken { get; set; }
}