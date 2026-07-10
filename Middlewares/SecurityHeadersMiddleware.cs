namespace ProductManagement.Middlewares;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Prevent browser interpreting files as something other than declared
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // Protect against Clickjacking forbidding the site from being framed inside another site
        context.Response.Headers.Append("X-Frame-Options", "Deny");

        // Enable built-in Cross-Site Scripting (XSS) protections in modern legacy browsers
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Pass execution to next middleware component
        await _next(context);
    }
}
