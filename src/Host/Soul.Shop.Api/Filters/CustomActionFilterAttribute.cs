using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Soul.Shop.Infrastructure;

namespace Soul.Shop.Api.Filters;

public class CustomActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var result = Result.Fail("Please login again.");
            context.Result = new JsonResult(result);
        }
        else if (context.HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            var result = Result.Fail("You have no permission to access.");
            context.Result = new JsonResult(result);
        }
        else
        {
            if (!context.ModelState.IsValid)
            {
                var error = context.ModelState.Values.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage ??
                            "Parameter exception.";
                context.Result = new JsonResult(Result.Fail(error));
            }
        }

        base.OnActionExecuting(context);
    }
}