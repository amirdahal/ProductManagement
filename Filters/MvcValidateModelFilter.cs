using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductManagement.Filters
{
    public class MvcValidateModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        // Runs AFTER the controller compiles data but BEFORE the HTML is rendered
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;

            if (controller != null && !controller.ModelState.IsValid)
            {
                // If the controller tried to redirect the user away on failure, 
                // stop it and force the original view form to re-render with errors illuminated.
                if (context.Result is RedirectToActionResult || context.Result is RedirectToRouteResult)
                {
                    var actionName = context.RouteData.Values["action"]?.ToString() ?? "Index";

                    // Force the original view to display the posted form model
                    context.Result = controller.View(actionName);
                }
            }
        }
    }
}