using Shop.Module.BasicAuth;

namespace Soul.Shop.Module.Auth;

public class BasicAuthAuthorizationFilterOptions
{
    public BasicAuthAuthorizationFilterOptions()
    {
        SslRedirect = true;
        RequireSsl = true;
        LoginCaseSensitive = true;
        Users = new BasicAuthAuthorizationUser[] { };
    }

    public bool SslRedirect { get; set; }

    public bool RequireSsl { get; set; }

    public bool LoginCaseSensitive { get; set; }

    public IEnumerable<BasicAuthAuthorizationUser> Users { get; set; }
}