using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ProductManagement.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred during action execution.");


        var errorResponse = new
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "A critical server error occurred. Please try again later.",
            Detailed = context.Exception.Message
        };

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
