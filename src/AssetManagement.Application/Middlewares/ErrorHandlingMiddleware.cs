using System.Text.Json;
using AssetManagement.Application.Extensions;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Exceptions;

namespace AssetManagement.Application.Middlewares;

/// <summary>
/// Middleware that handles exceptions occurring during the request pipeline execution.
/// Converts exceptions into formatted API responses with appropriate status codes.
/// </summary>
public class ErrorHandlingMiddleware
{
    /// <summary>
    /// Represents the logger instance used within the <see cref="ErrorHandlingMiddleware"/> class
    /// to log information, warnings, and errors, including handling exception details.
    /// It is specifically implemented as an <see cref="ILogger{ErrorHandlingMiddleware}"/>
    /// to provide typed logging functionality for middleware operations.
    /// </summary>
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Represents the next middleware in the request pipeline.
    /// Used to pass the HTTP context to the subsequent middleware for further processing.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Middleware that handles exceptions globally in the application, ensuring consistent error responses and logging.
    /// </summary>
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Handles HTTP requests by invoking the next middleware in the pipeline and catches exceptions
    /// to process them into a structured error response.
    /// </summary>
    /// <param name="context">The HTTP context representing the current request and response.</param>
    /// <returns>A task representing the asynchronous operation for processing the HTTP request.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// Handles exceptions that occur during the processing of a request and generates appropriate
    /// responses to return to the client. This method logs the exception, determines the appropriate
    /// status code, and serializes a standardized error response to the client.
    /// <param name="context">The current HTTP context associated with the request.</param>
    /// <param name="exception">The exception that occurred during request processing.</param>
    /// <returns>A task that represents the asynchronous operation of handling the exception and
    /// responding to the client.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        ApiResponse<object> response;

        if (exception is AppException ex)
            response = ApiResponseExtensions.ToErrorResponse<object>(ex.Message,
                [$"{ex.ErrorCode}: {ex.Message}"]);
        else if (exception is KeyNotFoundException exKnf)
            response = ApiResponseExtensions.ToNotFoundResponse<object>(exKnf.Message);
        else
            response = ApiResponseExtensions.ToErrorResponse<object>(
                "An unexpected error occurred",
                [exception.Message]);

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Determines the appropriate HTTP status code based on the provided exception.
    /// </summary>
    /// <param name="exception">The exception to evaluate for determining the status code.</param>
    /// <returns>The corresponding HTTP status code as an integer. Returns 404 for <see cref="KeyNotFoundException"/>,
    /// the status code from the <see cref="AppException"/> instance, or 500 for all other exceptions.</returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            AppException ex => ex.StatusCode,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}