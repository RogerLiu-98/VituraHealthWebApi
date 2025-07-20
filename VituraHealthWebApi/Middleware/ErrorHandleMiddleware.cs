using System.Net;
using System.Text.Json;

namespace VituraHealthWebApi.Middleware;

public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandleMiddleware> _logger;

    public ErrorHandleMiddleware(RequestDelegate next, ILogger<ErrorHandleMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context != null && context.Response != null && exception != null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Set the status code to 500

            await context.Response.WriteAsync("Internal Server Error");
        }
    }
}

// Extension method for easier registration
public static class ErrorHandleMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandleMiddleware>();
    }
}
