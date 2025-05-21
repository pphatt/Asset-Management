using Microsoft.AspNetCore.Http;

namespace AssetManagement.Contracts.Exceptions;

/// <summary>
/// Provides a collection of custom exception types that extend the <see cref="AppException"/> base class.
/// Each exception is tailored to represent specific error scenarios commonly encountered in an API context,
/// such as client errors (e.g., Bad Request, Unauthorized) or server errors.
///
/// <para>Key Features:</para>
/// <list type="bullet">
/// <item>
/// <term>NotFoundException</term>
/// <description>Represents a 404 Not Found exception, used when a requested resource cannot be found.</description>
/// </item>
/// <item>
/// <term>BadRequestException</term>
/// <description>Represents a 400 Bad Request exception for invalid client requests.</description>
/// </item>
/// <item>
/// <term>UnauthorizedException</term>
/// <description>Represents a 401 Unauthorized exception, used when authentication is required but not provided or invalid.</description>
/// </item>
/// <item>
/// <term>ForbiddenException</term>
/// <description>Represents a 403 Forbidden exception for scenarios where the client lacks appropriate permissions.</description>
/// </item>
/// <item>
/// <term>ConflictException</term>
/// <description>Represents a 409 Conflict exception, used for conflicts such as resource duplication.</description>
/// </item>
/// <item>
/// <term>ServerErrorException</term>
/// <description>Represents a 500 Internal Server Error exception for unexpected server errors.</description>
/// </item>
/// <item>
/// <term>ValidationException</term>
/// <description>A specialized exception for invalid input that extends <see cref="BadRequestException"/>.</description>
/// </item>
/// <item>
/// <term>DuplicateResourceException</term>
/// <description>A specialized exception for resource duplication that extends <see cref="ConflictException"/>.</description>
/// </item>
/// <item>
/// <term>ResourceAlreadyExistsException</term>
/// <description>A specialized exception for already existing resources that extends <see cref="ConflictException"/>.</description>
/// </item>
/// </list>
///
/// <para>Usage:</para>
/// These exception types provide meaningful error differentiation both for internal logging 
/// and client-facing error responses, consistently embedding HTTP status codes and error codes.
/// </summary>
public static class ApiExceptionTypes
{
    /// <summary>
    /// Represents a specialized exception for scenarios where a requested resource is not found,
    /// corresponding to an HTTP 404 Not Found status code.
    /// This exception is commonly used to indicate that a specific entity or resource,
    /// identified by criteria such as an ID or a name, could not be located within the system.
    /// </summary>
    public class NotFoundException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when a requested resource is not found.
        /// </summary>
        /// <remarks>
        /// This exception is used to indicate a 404 Not Found error, typically when a resource
        /// requested by the client is not available.
        /// </remarks>
        /// <example>
        /// Use this exception in scenarios where a specific entity or data could not be located,
        /// such as when querying for a nonexistent database record.
        /// </example>
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound, "NOT_FOUND")
        {
        }
    }

    /// <summary>
    /// Represents a 400 Bad Request exception, indicating that the client's request is invalid or malformed.
    /// This exception is part of the <see cref="ApiExceptionTypes"/> collection, designed to handle specific API error scenarios.
    /// <para>Purpose:</para>
    /// To provide a standardized way to handle and communicate client-side errors related to invalid requests.
    /// Examples include invalid input data, missing required parameters, or improperly formatted requests.
    /// <para>Key Features:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Status Code</term>
    /// <description>Returns HTTP status code 400 (Bad Request) to indicate a client-side error.</description>
    /// </item>
    /// <item>
    /// <term>Error Code</term>
    /// <description>Includes the error code "BAD_REQUEST" for consistent issue identification.</description>
    /// </item>
    /// <item>
    /// <term>Customizable Message</term>
    /// <description>Accepts a user-defined message to provide context about the specific validation or error issue.</description>
    /// </item>
    /// </list>
    /// </summary>
    public class BadRequestException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when a bad request is made to the application.
        /// </summary>
        /// <remarks>
        /// The exception is used to indicate situations where the client has made an invalid or malformed request,
        /// or when the server cannot fulfill the request due to incorrect data provided by the client.
        /// It results in an HTTP 400 Bad Request response.
        /// </remarks>
        /// <example>
        /// Common scenarios for throwing this exception include invalid input data, missing query parameters,
        /// or malformed request payloads.
        /// </example>
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest, "BAD_REQUEST")
        {
        }
    }

    /// <summary>
    /// Represents a custom exception class for 401 Unauthorized error scenarios, extending the <see cref="AppException"/> base class.
    /// This exception is designed to signal that authentication is required and has either not been provided
    /// or is invalid. It is commonly used in API contexts where authorization is necessary for accessing
    /// certain resources or performing specific actions.
    /// Key Features:
    /// - Extends the <see cref="AppException"/> base class to provide consistency in exception handling.
    /// - Automatically sets the HTTP status code to 401 (Unauthorized).
    /// - Includes a unique error code "UNAUTHORIZED" for easy identification.
    /// Use Case:
    /// Intended for scenarios where the client is unauthenticated, such as missing or invalid credentials
    /// in the API request.
    /// Example:
    /// When thrown, this exception can be used to indicate that access to a resource requires proper
    /// authentication and authorization.
    /// </summary>
    public class UnauthorizedException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when an unauthorized access attempt is detected.
        /// </summary>
        /// <remarks>
        /// This exception is used to indicate a 401 Unauthorized error, typically when a client attempts
        /// to access a resource without proper authentication or permission.
        /// </remarks>
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized, "UNAUTHORIZED")
        {
        }
    }

    /// <summary>
    /// Represents a 403 Forbidden exception, indicating that the client does not have the necessary permissions to access the requested resource or perform the requested operation.
    /// This exception is tailored for scenarios where authentication may have succeeded, but the authenticated user or entity is not authorized to perform the action.
    /// <para>Key Features:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Status Code:</term>
    /// <description>403 Forbidden</description>
    /// </item>
    /// <item>
    /// <term>Error Code:</term>
    /// <description>FORBIDDEN</description>
    /// </item>
    /// <item>
    /// <term>Message:</term>
    /// <description>Allows customization of the error message to describe the specific reason for denial.</description>
    /// </item>
    /// </list>
    /// <para>Use Case:</para>
    /// Typically used within APIs or application layers to enforce authorization rules and communicate forbidden access scenarios to clients.
    /// </summary>
    public class ForbiddenException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when a user does not have the required permissions or access to perform a specific action or access a resource.
        /// </summary>
        /// <remarks>
        /// This exception corresponds to HTTP status code 403 (Forbidden) and indicates that the request was valid,
        /// but the server is refusing action due to lack of permissions or access rights.
        /// </remarks>
        public ForbiddenException(string message)
            : base(message, StatusCodes.Status403Forbidden, "FORBIDDEN")
        {
        }
    }

    /// <summary>
    /// Represents a 409 Conflict exception, used for scenarios where a conflict occurs in the application,
    /// such as when creating a resource that already exists or updating a resource that leads to a logical conflict.
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Status Code</term>
    /// <description>Returns a 409 Conflict HTTP status code to indicate the conflict error.</description>
    /// </item>
    /// <item>
    /// <term>Error Identifier</term>
    /// <description>Provides "CONFLICT" as a consistent error type identifier.</description>
    /// </item>
    /// </list>
    /// This exception is part of the <see cref="ApiExceptionTypes"/> collection and extends the <see cref="AppException"/> class
    /// to provide consistent and meaningful error handling for conflict scenarios in the application context.
    /// </summary>
    public class ConflictException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when a conflict occurs during the execution of a request.
        /// </summary>
        /// <remarks>
        /// This exception indicates a 409 Conflict HTTP status code and is used to signal that the request could not be completed due to a conflict with the current state of the target resource.
        /// </remarks>
        protected ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict, "CONFLICT")
        {
        }
    }

    /// <summary>
    /// Represents a specific type of exception used to indicate a conflict scenario
    /// where a resource duplication has occurred in the application.
    /// This exception extends the <see cref="ConflictException"/> base class to provide additional semantic meaning
    /// for cases involving duplicate resource issues, typically associated with HTTP status code 409 (Conflict).
    /// </summary>
    /// <remarks>
    /// This exception is primarily designed for use cases where a resource being created or modified
    /// already exists in the system, violating constraints or uniqueness rules.
    /// It is part of the API's custom exception framework, enabling clear and consistent error reporting.
    /// </remarks>
    public class DuplicateResourceException : ConflictException
    {
        /// <summary>
        /// Represents an exception that is thrown when a duplicate resource is encountered.
        /// </summary>
        /// <remarks>
        /// This exception indicates a conflict scenario where an attempt to create or add a resource
        /// fails due to an already existing resource with the same identity or characteristics.
        /// </remarks>
        /// <example>
        /// Suitable for scenarios where operations like adding a resource to a database violate duplication constraints.
        /// </example>
        public DuplicateResourceException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Represents an exception that is thrown when an attempt is made to create or add a resource
    /// that already exists in the system. This exception extends the <see cref="ConflictException"/>
    /// to specifically handle scenarios where resource duplication conflicts occur.
    /// Key Features:
    /// - Provides a clear, descriptive error for resource duplication attempts.
    /// - Adheres to the conflict pattern (409 Conflict) in API responses.
    /// </summary>
    public class ResourceAlreadyExistsException : ConflictException
    {
        /// <summary>
        /// Represents an exception that is thrown when an attempt is made to create a resource
        /// that already exists.
        /// </summary>
        /// <remarks>
        /// This exception is typically used to indicate a 409 Conflict error in scenarios
        /// where a client tries to add a duplicate resource that violates a uniqueness constraint.
        /// </remarks>
        public ResourceAlreadyExistsException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Represents a specialized exception for input validation errors in the application.
    /// Extends the <see cref="BadRequestException"/> to indicate that invalid input was provided
    /// by the client, resulting in a 400 Bad Request response.
    /// <para>Key Features:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Error Specification</term>
    /// <description>Used to capture and communicate issues related to input validation, such as missing or improperly formatted data.</description>
    /// </item>
    /// <item>
    /// <term>Extends BadRequestException</term>
    /// <description>Inherits from <see cref="BadRequestException"/> to utilize its behavior and explicitly categorize the error.</description>
    /// </item>
    /// <item>
    /// <term>Customizability</term>
    /// <description>Accepts a message parameter to detail the specific validation issue encountered.</description>
    /// </item>
    /// </list>
    /// This exception is typically thrown in scenarios where input data does not conform to expected rules
    /// or validations, helping enforce data integrity and safe operation of the application.
    /// </summary>
    public class ValidationException : BadRequestException
    {
        /// <summary>
        /// Represents an exception that is thrown when input validation fails.
        /// </summary>
        /// <remarks>
        /// This exception is typically used to indicate that one or more of the provided
        /// inputs to a method or request do not satisfy the expected validation rules.
        /// It results in a 400 Bad Request response.
        /// </remarks>
        public ValidationException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Represents a 500 Internal Server Error exception, indicating that an unexpected error occurred on the server side.
    /// Typically used to signify unhandled or critical issues that prevent the server from fulfilling the request.
    /// <para>Details:</para>
    /// This exception extends the <see cref="AppException"/> base class,
    /// providing a standardized way to report server-side errors with a specific status code and error message.
    /// <para>Key Characteristics:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Status Code</term>
    /// <description>Uses HTTP Status Code 500 to indicate an internal server error.</description>
    /// </item>
    /// <item>
    /// <term>Error Code</term>
    /// <description>Default error code is "SERVER_ERROR" for easier identification in logs and debugging.</description>
    /// </item>
    /// <item>
    /// <term>Message</term>
    /// <description>Customizable error message provided at the time of exception creation, detailing the specific issue encountered.</description>
    /// </item>
    /// </list>
    /// Typically invoked when the server encounters an error that does not fall into predefined client or resource-related exceptions.
    /// </summary>
    public class ServerErrorException : AppException
    {
        /// <summary>
        /// Represents an exception that is thrown when a server-side error occurs.
        /// </summary>
        /// <remarks>
        /// This exception is typically used to indicate a 500 Internal Server Error, signifying
        /// that an unexpected condition prevented the server from fulfilling a request.
        /// </remarks>
        public ServerErrorException(string message)
            : base(message, StatusCodes.Status500InternalServerError, "SERVER_ERROR")
        {
        }
    }
}