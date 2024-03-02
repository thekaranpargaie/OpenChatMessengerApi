using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Base.Filters
{
    public class ApiResultFilterAttribute : ResultFilterAttribute
    {
        public override async void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorMessage = context.ModelState.Values.SelectMany(v=>v.Errors).Select(v => v.ErrorMessage).ToList();
                var str = String.Join(", ", errorMessage);
                context.Result = new JsonResult(await Result.FailAsync(str))
                {
                    StatusCode = 200
                };
                return;
            }
        }
    }
}
