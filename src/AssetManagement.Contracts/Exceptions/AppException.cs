namespace AssetManagement.Contracts.Exceptions;
public class AppException : Exception
{
    /// <summary>
    /// Represents a custom application exception for handling errors within the Asset Management application.
    /// This exception encapsulates additional context, such as HTTP status codes and error codes, 
    /// to provide more meaningful error information for logging and client communication.
    ///
    /// <para>Properties:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>StatusCode</term>
    /// <description>The HTTP status code associated with the error (default: 500).</description>
    /// </item>
    /// <item>
    /// <term>ErrorCode</term>
    /// <description>A custom error code to describe the specific error scenario (default: "GENERAL_ERROR").</description>
    /// </item>
    /// </list>
    ///
    /// <para>Usage:</para>
    /// Throw this exception to propagate domain-specific errors with clear context, 
    /// enabling consistent error handling across the application.
    /// </summary>    /// <inheritdoc />
    public AppException(string message, int statusCode = 500, string errorCode = "GENERAL_ERROR")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public int StatusCode { get; }
    public string ErrorCode { get; }
}