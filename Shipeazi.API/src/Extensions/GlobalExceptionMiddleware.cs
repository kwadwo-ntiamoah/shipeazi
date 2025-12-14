using Microsoft.AspNetCore.Mvc;

namespace Shipeazi.API.src.Extensions
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception caught by GlobalExceptionMiddleware: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Detail = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again later.",
                Instance = context.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            // Add stack trace in development mode
            if (environment.IsDevelopment())
            {
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;
                problemDetails.Extensions["exception"] = exception.GetType().Name;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}