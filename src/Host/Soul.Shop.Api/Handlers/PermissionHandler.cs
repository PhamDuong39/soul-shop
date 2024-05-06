using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Soul.Shop.Module.Core.Abstractions.Extensions;

namespace Soul.Shop.Api.Handlers;

public class PermissionHandler(IWorkContext workContext, IHttpContextAccessor httpContextAccessor)
    : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var isAuthenticated = context.User?.Identity?.IsAuthenticated;
        if (isAuthenticated == true)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

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