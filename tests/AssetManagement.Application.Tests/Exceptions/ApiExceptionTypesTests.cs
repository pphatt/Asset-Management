using AssetManagement.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AssetManagement.Tests.Exceptions;

public class ApiExceptionTypesTests
{
    #region NotFoundException Tests

    [Fact]
    public void NotFoundException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Resource not found";

        // Act
        var exception = new ApiExceptionTypes.NotFoundException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void NotFoundException_ShouldSetCorrectStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.NotFoundException(message);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
    }

    [Fact]
    public void NotFoundException_ShouldSetCorrectErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.NotFoundException(message);

        // Assert
        Assert.Equal("NOT_FOUND", exception.ErrorCode);
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromAppException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.NotFoundException(message);

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    #endregion

    #region BadRequestException Tests

    [Fact]
    public void BadRequestException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Bad request";

        // Act
        var exception = new ApiExceptionTypes.BadRequestException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void BadRequestException_ShouldSetCorrectStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.BadRequestException(message);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
    }

    [Fact]
    public void BadRequestException_ShouldSetCorrectErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.BadRequestException(message);

        // Assert
        Assert.Equal("BAD_REQUEST", exception.ErrorCode);
    }

    [Fact]
    public void BadRequestException_ShouldInheritFromAppException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.BadRequestException(message);

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    #endregion

    #region UnauthorizedException Tests

    [Fact]
    public void UnauthorizedException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Unauthorized access";

        // Act
        var exception = new ApiExceptionTypes.UnauthorizedException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void UnauthorizedException_ShouldSetCorrectStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.UnauthorizedException(message);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, exception.StatusCode);
    }

    [Fact]
    public void UnauthorizedException_ShouldSetCorrectErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.UnauthorizedException(message);

        // Assert
        Assert.Equal("UNAUTHORIZED", exception.ErrorCode);
    }

    [Fact]
    public void UnauthorizedException_ShouldInheritFromAppException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.UnauthorizedException(message);

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    #endregion

    #region ForbiddenException Tests

    [Fact]
    public void ForbiddenException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Access forbidden";

        // Act
        var exception = new ApiExceptionTypes.ForbiddenException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ForbiddenException_ShouldSetCorrectStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ForbiddenException(message);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, exception.StatusCode);
    }

    [Fact]
    public void ForbiddenException_ShouldSetCorrectErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ForbiddenException(message);

        // Assert
        Assert.Equal("FORBIDDEN", exception.ErrorCode);
    }

    [Fact]
    public void ForbiddenException_ShouldInheritFromAppException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ForbiddenException(message);

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    #endregion

    #region ConflictException Tests

    [Fact]
    public void ConflictException_ShouldSetCorrectStatusCode()
    {
        // Arrange & Act
        var exception = new TestableConflictException("Test message");

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
    }

    [Fact]
    public void ConflictException_ShouldSetCorrectErrorCode()
    {
        // Arrange & Act
        var exception = new TestableConflictException("Test message");

        // Assert
        Assert.Equal("CONFLICT", exception.ErrorCode);
    }

    [Fact]
    public void ConflictException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Conflict occurred";

        // Act
        var exception = new TestableConflictException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ConflictException_ShouldInheritFromAppException()
    {
        // Arrange & Act
        var exception = new TestableConflictException("Test message");

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    // Helper class to test protected ConflictException
    private class TestableConflictException : ApiExceptionTypes.ConflictException
    {
        public TestableConflictException(string message) : base(message)
        {
        }
    }

    #endregion

    #region DuplicateResourceException Tests

    [Fact]
    public void DuplicateResourceException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Duplicate resource found";

        // Act
        var exception = new ApiExceptionTypes.DuplicateResourceException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void DuplicateResourceException_ShouldInheritFromConflictException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.DuplicateResourceException(message);

        // Assert
        Assert.IsAssignableFrom<ApiExceptionTypes.ConflictException>(exception);
    }

    [Fact]
    public void DuplicateResourceException_ShouldHaveConflictStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.DuplicateResourceException(message);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
    }

    [Fact]
    public void DuplicateResourceException_ShouldHaveConflictErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.DuplicateResourceException(message);

        // Assert
        Assert.Equal("CONFLICT", exception.ErrorCode);
    }

    #endregion

    #region ResourceAlreadyExistsException Tests

    [Fact]
    public void ResourceAlreadyExistsException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Resource already exists";

        // Act
        var exception = new ApiExceptionTypes.ResourceAlreadyExistsException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ResourceAlreadyExistsException_ShouldInheritFromConflictException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ResourceAlreadyExistsException(message);

        // Assert
        Assert.IsAssignableFrom<ApiExceptionTypes.ConflictException>(exception);
    }

    [Fact]
    public void ResourceAlreadyExistsException_ShouldHaveConflictStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ResourceAlreadyExistsException(message);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, exception.StatusCode);
    }

    [Fact]
    public void ResourceAlreadyExistsException_ShouldHaveConflictErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ResourceAlreadyExistsException(message);

        // Assert
        Assert.Equal("CONFLICT", exception.ErrorCode);
    }

    #endregion

    #region ValidationException Tests

    [Fact]
    public void ValidationException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Validation failed";

        // Act
        var exception = new ApiExceptionTypes.ValidationException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ValidationException_ShouldInheritFromBadRequestException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ValidationException(message);

        // Assert
        Assert.IsAssignableFrom<ApiExceptionTypes.BadRequestException>(exception);
    }

    [Fact]
    public void ValidationException_ShouldHaveBadRequestStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ValidationException(message);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, exception.StatusCode);
    }

    [Fact]
    public void ValidationException_ShouldHaveBadRequestErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ValidationException(message);

        // Assert
        Assert.Equal("BAD_REQUEST", exception.ErrorCode);
    }

    #endregion

    #region ServerErrorException Tests

    [Fact]
    public void ServerErrorException_ShouldSetCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Internal server error";

        // Act
        var exception = new ApiExceptionTypes.ServerErrorException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ServerErrorException_ShouldSetCorrectStatusCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ServerErrorException(message);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, exception.StatusCode);
    }

    [Fact]
    public void ServerErrorException_ShouldSetCorrectErrorCode()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ServerErrorException(message);

        // Assert
        Assert.Equal("SERVER_ERROR", exception.ErrorCode);
    }

    [Fact]
    public void ServerErrorException_ShouldInheritFromAppException()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ApiExceptionTypes.ServerErrorException(message);

        // Assert
        Assert.IsAssignableFrom<AppException>(exception);
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData(typeof(ApiExceptionTypes.NotFoundException), StatusCodes.Status404NotFound, "NOT_FOUND")]
    [InlineData(typeof(ApiExceptionTypes.BadRequestException), StatusCodes.Status400BadRequest, "BAD_REQUEST")]
    [InlineData(typeof(ApiExceptionTypes.UnauthorizedException), StatusCodes.Status401Unauthorized, "UNAUTHORIZED")]
    [InlineData(typeof(ApiExceptionTypes.ForbiddenException), StatusCodes.Status403Forbidden, "FORBIDDEN")]
    [InlineData(typeof(ApiExceptionTypes.ServerErrorException), StatusCodes.Status500InternalServerError, "SERVER_ERROR")]
    public void AllExceptions_ShouldHaveCorrectStatusCodeAndErrorCode(Type exceptionType, int expectedStatusCode, string expectedErrorCode)
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = (AppException)Activator.CreateInstance(exceptionType, message)!;

        // Assert
        Assert.Equal(expectedStatusCode, exception.StatusCode);
        Assert.Equal(expectedErrorCode, exception.ErrorCode);
        Assert.Equal(message, exception.Message);
    }

    [Theory]
    [InlineData(typeof(ApiExceptionTypes.ValidationException), typeof(ApiExceptionTypes.BadRequestException))]
    [InlineData(typeof(ApiExceptionTypes.DuplicateResourceException), typeof(ApiExceptionTypes.ConflictException))]
    [InlineData(typeof(ApiExceptionTypes.ResourceAlreadyExistsException), typeof(ApiExceptionTypes.ConflictException))]
    public void SpecializedExceptions_ShouldInheritFromCorrectBaseException(Type specializedType, Type baseType)
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = Activator.CreateInstance(specializedType, message);

        // Assert
        Assert.IsAssignableFrom(baseType, exception);
    }

    [Fact]
    public void AllPublicExceptions_ShouldBeInstantiableWithMessage()
    {
        // Arrange
        const string testMessage = "Test exception message";
        var exceptionTypes = new[]
        {
            typeof(ApiExceptionTypes.NotFoundException),
            typeof(ApiExceptionTypes.BadRequestException),
            typeof(ApiExceptionTypes.UnauthorizedException),
            typeof(ApiExceptionTypes.ForbiddenException),
            typeof(ApiExceptionTypes.DuplicateResourceException),
            typeof(ApiExceptionTypes.ResourceAlreadyExistsException),
            typeof(ApiExceptionTypes.ValidationException),
            typeof(ApiExceptionTypes.ServerErrorException)
        };

        foreach (var exceptionType in exceptionTypes)
        {
            // Act
            var exception = (AppException)Activator.CreateInstance(exceptionType, testMessage)!;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(testMessage, exception.Message);
            Assert.True(exception.StatusCode > 0);
            Assert.False(string.IsNullOrEmpty(exception.ErrorCode));
        }
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Exceptions_ShouldHandleLongMessages()
    {
        // Arrange
        var longMessage = new string('A', 10000);

        // Act
        var exception = new ApiExceptionTypes.NotFoundException(longMessage);

        // Assert
        Assert.Equal(longMessage, exception.Message);
        Assert.Equal(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.Equal("NOT_FOUND", exception.ErrorCode);
    }

    #endregion
}