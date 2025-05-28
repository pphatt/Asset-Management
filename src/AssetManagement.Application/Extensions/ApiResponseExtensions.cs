using AssetManagement.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Extensions;

/// <summary>
/// Provides extension methods for creating and returning standardized API response objects
/// using the ApiResponse class. Supports success, error, and various HTTP status responses.
/// </summary>
public static class ApiResponseExtensions
{
    /// <summary>
    /// Converts the specified data into a successful API response object.
    /// </summary>
    /// <typeparam name="T">The type of the data to include in the response.</typeparam>
    /// <param name="data">The data to be included in the response.</param>
    /// <param name="message">The success message to include in the response. Defaults to "Operation completed successfully".</param>
    /// <returns>An <see cref="ApiResponse{T}"/> object with a success status, including the specified data and message.</returns>
    internal static ApiResponse<T> ToSuccessResponse<T>(this T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    /// <summary>
    /// Converts the data instance into an ApiResponse object representing a successful "Created" response.
    /// </summary>
    /// <typeparam name="T">The type of the data being included in the response.</typeparam>
    /// <param name="data">The data to be included in the response.</param>
    /// <param name="message">An optional success message. Defaults to "Resource created successfully".</param>
    /// <returns>An ApiResponse object containing the provided data, a success flag set to true, the provided message, and an empty list of errors.</returns>
    internal static ApiResponse<T> ToCreatedResponse<T>(this T data, string message = "Resource created successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    /// Generates an `ApiResponse` instance indicating a "not found" scenario.
    /// <typeparam name="T">The type of the data to be encapsulated in the response.</typeparam>
    /// <param name="message">The message to include in the response. Defaults to "Resource not found".</param>
    /// <returns>An `ApiResponse` with the `Success` property set to false, a default `Data` value,
    /// and the provided message added to the `Errors` list.</returns>
    internal static ApiResponse<T> ToNotFoundResponse<T>(string message = "Resource not found")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = [message]
        };
    }

    /// <summary>
    /// Creates a bad request ApiResponse object indicating the operation was unsuccessful.
    /// </summary>
    /// <typeparam name="T">The type of the data to be included in the response.</typeparam>
    /// <param name="message">A descriptive message about the bad request.</param>
    /// <param name="errors">A list of error messages associated with the bad request. Defaults to a list containing the provided message.</param>
    /// <returns>An ApiResponse object with the success status set to false, including the message and any provided errors.</returns>
    internal static ApiResponse<T> ToBadRequestResponse<T>(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? [message]
        };
    }

    /// <summary>
    /// Creates an ApiResponse object representing a conflict response with the provided message and optional list of errors.
    /// </summary>
    /// <typeparam name="T">The type of data associated with the response.</typeparam>
    /// <param name="message">The message describing the conflict.</param>
    /// <param name="errors">Optional list of additional error details. If null, the message is used as the only error detail.</param>
    /// <returns>An ApiResponse object containing the conflict information.</returns>
    internal static ApiResponse<T> ToConflictResponse<T>(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? [message]
        };
    }

    /// Converts a provided error message and optional list of errors into an ApiResponse object
    /// representing a failed operation.
    /// <typeparam name="T">The type of the data expected in the response.</typeparam>
    /// <param name="message">The primary message describing the error.</param>
    /// <param name="errors">An optional list of specific error details.</param>
    /// <returns>An ApiResponse object with Success set to false, the provided error message, and optional errors.</returns>
    public static ApiResponse<T> ToErrorResponse<T>(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? [message]
        };
    }

    // Controller extension methods for returning API responses
    /// Converts the provided data into an ActionResult containing an API success response.
    /// <typeparam name="T">The type of the data being returned.</typeparam>
    /// <param name="controller">The controller instance invoking the method.</param>
    /// <param name="data">The data to be included in the API response.</param>
    /// <param name="message">An optional success message to be included in the API response. Defaults to "Operation completed successfully".</param>
    /// <returns>An ActionResult containing the API response with the success status, message, and data.</returns>
    public static ActionResult<ApiResponse<T>> ToApiResponse<T>(this ControllerBase controller, T data, string message = "Operation completed successfully")
    {
        return controller.Ok(data.ToSuccessResponse(message));
    }

    /// <summary>
    /// Creates an HTTP 201 Created response with a standard API response structure, including the provided data and message.
    /// </summary>
    /// <typeparam name="T">The type of the data to include in the response.</typeparam>
    /// <param name="controller">The controller instance from which this method is invoked.</param>
    /// <param name="data">The data to include in the response.</param>
    /// <param name="message">An optional message to include in the response. Defaults to "Resource created successfully".</param>
    /// <returns>An ActionResult containing an ApiResponse of type <typeparamref name="T"/> with HTTP status code 201 Created.</returns>
    public static ActionResult<ApiResponse<T>> ToCreatedApiResponse<T>(this ControllerBase controller, T data, string message = "Resource created successfully")
    {
        return controller.StatusCode(StatusCodes.Status201Created, data.ToCreatedResponse(message));
    }

    /// Converts the response to a standardized "not found" API response and returns it as an ActionResult.
    /// <typeparam name="T">The type of the data in the API response.</typeparam>
    /// <param name="controller">The ControllerBase instance calling this method.</param>
    /// <param name="message">The message describing the "not found" condition. Defaults to "Resource not found".</param>
    /// <returns>An ActionResult containing an ApiResponse indicating that the requested resource was not found.</returns>
    public static ActionResult<ApiResponse<T>> ToNotFoundApiResponse<T>(this ControllerBase controller, string message = "Resource not found")
    {
        return controller.NotFound(ToNotFoundResponse<T>(message));
    }

    /// <summary>
    /// Converts a bad request response into an <see cref="ApiResponse{T}"/> object and returns it as an <see cref="ActionResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    /// <param name="controller">The controller that invokes this method.</param>
    /// <param name="message">The message describing the bad request reason.</param>
    /// <param name="errors">An optional list of error messages providing additional details about the bad request.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the <see cref="ApiResponse{T}"/> for the bad request.</returns>
    public static ActionResult<ApiResponse<T>> ToBadRequestApiResponse<T>(this ControllerBase controller, string message, List<string>? errors = null)
    {
        return controller.BadRequest(ToBadRequestResponse<T>(message, errors));
    }

    /// <summary>
    /// Returns an HTTP 409 Conflict response with a structured API response containing a message and optional error details.
    /// </summary>
    /// <typeparam name="T">The type of the data relevant to the API response.</typeparam>
    /// <param name="controller">The controller instance from which the method is invoked.</param>
    /// <param name="message">The message describing the conflict that occurred.</param>
    /// <param name="errors">An optional list of error details related to the conflict.</param>
    /// <returns>An ActionResult wrapping an ApiResponse of type T with HTTP status 409 (Conflict).</returns>
    public static ActionResult<ApiResponse<T>> ToConflictApiResponse<T>(this ControllerBase controller, string message, List<string>? errors = null)
    {
        return controller.StatusCode(StatusCodes.Status409Conflict, ToConflictResponse<T>(message, errors));
    }

    /// <summary>
    /// Generates an internal server error API response (500) with the specified message and optional error details.
    /// </summary>
    /// <typeparam name="T">The type of the data to be included in the response.</typeparam>
    /// <param name="controller">The controller instance from which the response is generated.</param>
    /// <param name="message">The message describing the error.</param>
    /// <param name="errors">A list of specific errors associated with the response. Defaults to null.</param>
    /// <returns>An ActionResult containing an ApiResponse with a 500 status code.</returns>
    public static ActionResult<ApiResponse<T>> ToErrorApiResponse<T>(this ControllerBase controller, string message, List<string>? errors = null)
    {
        return controller.StatusCode(StatusCodes.Status500InternalServerError, ToErrorResponse<T>(message, errors));
    }
}
