using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shop.Module.Core.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.WebApi.Handlers;

public class PermissionHandler(IWorkContext workContext, IHttpContextAccessor httpContextAccessor)
    : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var isAuthenticated = context.User?.Identity?.IsAuthenticated;
        if (isAuthenticated == true)
        {
            // Lưu ý: Mã thông báo không hợp lệ và cần phải trả lại 401. Trả về 403 theo mặc định
            //if (context.Resource is AuthorizationFilterContext)
            //{
            //    var httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;
            //}

            var httpContext = httpContextAccessor?.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            // https://stackoverflow.com/questions/51119926/jwt-authentication-usermanager-getuserasync-returns-null
            // default the value of UserIdClaimType is ClaimTypes.NameIdentifier
            var identityId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            string token = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(token)) token = httpContext.Request.Query["access_token"];

            if (string.IsNullOrWhiteSpace(identityId) || string.IsNullOrWhiteSpace(token) ||
                !int.TryParse(identityId, out var userId) || userId <= 0)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                string path;
                if (httpContext.GetEndpoint() is RouteEndpoint endpoint && endpoint != null)
                    path = endpoint.RoutePattern.RawText;
                else
                    path = httpContext.Request?.Path.Value;

                path = $"{httpContext.Request.Method}:/{path?.Trim().Trim('/')}";

                // Mã thông báo được xác minh trong quá trình truy cập và tự động gia hạn
                if (!workContext.ValidateToken(userId,
                        token.Substring($"{JwtBearerDefaults.AuthenticationScheme} ".Length).Trim(), out var statusCode,
                        path))
                    if (statusCode != StatusCodes.Status403Forbidden)
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        await Task.CompletedTask;
    }
}
