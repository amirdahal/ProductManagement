using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductManagement.Filters;

public class ValidateModelFilter : IActionFilter
{

    private readonly IModelMetadataProvider _modeMetadataProvider;

    public ValidateModelFilter(IModelMetadataProvider modeMetadataProvider)
    {
        _modeMetadataProvider = modeMetadataProvider;
    }

    // Executes before the controller action runs
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // API only: Short-circuit with a flat 400 Bad Request JSON object
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
