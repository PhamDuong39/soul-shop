using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Soul.Shop.Module.ApiProfiler.Internal;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class ProfilingActionFilter : ActionFilterAttribute
{
    private const string StackKey = "ProfilingActionFilterStack";


    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var mp = MiniProfiler.Current;
        if (mp != null)
        {
            var stack = context.HttpContext.Items[StackKey] as Stack<IDisposable>;
            if (stack == null)
            {
                stack = new Stack<IDisposable>();
                context.HttpContext.Items[StackKey] = stack;
            }

            var area = context.RouteData.DataTokens.TryGetValue("area", out var areaToken)
                ? areaToken as string + "."
                : null;

            switch (context.ActionDescriptor)
            {
                case ControllerActionDescriptor cd:
                    if (mp.Name.IsNullOrWhiteSpace()) mp.Name = $"{cd.ControllerName}/{cd.MethodInfo.Name}";
                    stack.Push(mp.Step($"Controller: {area}{cd.ControllerName}.{cd.MethodInfo.Name}"));
                    break;
                case ActionDescriptor ad:
                    if (mp.Name.IsNullOrWhiteSpace()) mp.Name = ad.DisplayName;
                    stack.Push(mp.Step($"Controller: {area}{ad.DisplayName}"));
                    break;
            }
        }

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        if (context.HttpContext.Items[StackKey] is Stack<IDisposable> stack && stack.Count > 0) stack.Pop().Dispose();
    }
}