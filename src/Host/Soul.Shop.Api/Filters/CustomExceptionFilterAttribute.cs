using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Soul.Shop.Infrastructure;

namespace Soul.Shop.Api.Filters;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new JsonResult(Result.Fail(context.Exception.Message));
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        base.OnException(context);
    }
}
