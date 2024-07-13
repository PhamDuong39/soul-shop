using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shop.Infrastructure;
using System.Linq;

namespace Shop.WebApi.Filters;

public class CustomActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Vì PermissionHandler không thể trực tiếp trả về 401 trong thời điểm hiện tại nên nó tạm thời được điều chỉnh để xác minh 401 trước khi thực thi phương thức.
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var result = Result.Fail("Đăng nhập đã hết hạn, vui lòng đăng nhập lại");
            context.Result = new JsonResult(result);
        }
        else if (context.HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            var result = Result.Fail("Bạn không có quyền truy cập");
            context.Result = new JsonResult(result);
        }
        else
        {
            if (!context.ModelState.IsValid)
            {
                var error = context.ModelState.Values.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage ??
                            "Ngoại lệ tham số";
                context.Result = new JsonResult(Result.Fail(error));
            }
        }

        base.OnActionExecuting(context);
    }
}
