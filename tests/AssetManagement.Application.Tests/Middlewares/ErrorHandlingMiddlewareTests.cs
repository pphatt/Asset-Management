using System.Text.Json;
using AssetManagement.Application.Middlewares;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetManagement.Application.Tests.Middlewares;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock;
    private readonly ErrorHandlingMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;
    private readonly MemoryStream _responseBodyStream;

    public ErrorHandlingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        _middleware = new ErrorHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        _httpContext = new DefaultHttpContext();
        _responseBodyStream = new MemoryStream();
        _httpContext.Response.Body = _responseBodyStream;
    }

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenNoExceptionOccurs()
    {
        // Arrange
        _nextMock.Setup(next => next(_httpContext)).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once());
        _loggerMock.VerifyNoOtherCalls();
        Assert.Equal(0, _responseBodyStream.Length); // No response written
    }

    [Fact]
    public async Task InvokeAsync_HandlesAppException_Correctly()
    {
        // Arrange
        var exception = new AppException("Custom error", 400, "CUSTOM_ERROR");
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        var response = await GetResponseBody<ApiResponse<object>>();
        Assert.False(response.Success);
        Assert.Equal("Custom error", response.Message);
        Assert.Null(response.Data);
        Assert.Contains("CUSTOM_ERROR: Custom error", response.Errors);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Custom error")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_HandlesKeyNotFoundException_Correctly()
    {
        // Arrange
        var exception = new KeyNotFoundException("Resource not found");
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        var response = await GetResponseBody<ApiResponse<object>>();
        Assert.False(response.Success);
        Assert.Equal("Resource not found", response.Message);
        Assert.Null(response.Data);
        Assert.Contains("Resource not found", response.Errors);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Resource not found")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_HandlesAggregateFieldValidationException_Correctly()
    {
        // Arrange
        var validationErrors = new List<FieldValidationException>
        {
            new FieldValidationException("Name", "Name is required"),
            new FieldValidationException("CategoryId", "CategoryId is invalid")
        };
        var exception = new AggregateFieldValidationException(validationErrors);
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        var response = await GetResponseBody<ApiResponse<object>>();
        Assert.False(response.Success);
        Assert.Equal("Validation failed", response.Message);
        Assert.Null(response.Data);
        Assert.Contains("Name: Name is required", response.Errors);
        Assert.Contains("CategoryId: CategoryId is invalid", response.Errors);

        // No logging verification, as AggregateFieldValidationException returns early
    }

    [Fact]
    public async Task InvokeAsync_HandlesUnauthorizedAccessException_Correctly()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Unauthorized access");
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        var response = await GetResponseBody<ApiResponse<object>>();
        Assert.False(response.Success);
        Assert.Equal("An unexpected error occurred", response.Message);
        Assert.Null(response.Data);
        Assert.Contains("Unauthorized access", response.Errors);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unauthorized access")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_HandlesGenericException_Correctly()
    {
        // Arrange
        var exception = new Exception("Unexpected error");
        _nextMock.Setup(next => next(_httpContext)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        var response = await GetResponseBody<ApiResponse<object>>();
        Assert.False(response.Success);
        Assert.Equal("An unexpected error occurred", response.Message);
        Assert.Null(response.Data);
        Assert.Contains("Unexpected error", response.Errors);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unexpected error")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once());
    }

    private async Task<T> GetResponseBody<T>()
    {
        _responseBodyStream.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(_responseBodyStream).ReadToEndAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })!;
    }
}