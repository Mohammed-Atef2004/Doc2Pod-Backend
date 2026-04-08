using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Server Error";
        var detail = "An unexpected error occurred on our side.";
        var type = "Server.Error";

        IDictionary<string, string[]> validationErrors = null;

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Validation Error";
                detail = "One or more validation failures occurred.";
                type = "Server.Error";

                validationErrors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ValidationProblemDetails(validationErrors ?? new Dictionary<string, string[]>())
        {
            Status = (int)statusCode,
            Type = type,
            Title = title,
            Detail = detail
        };
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}