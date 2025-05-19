using Microsoft.AspNetCore.Http;

namespace AssetManagement.Contracts.Exceptions;

public static class ApiExceptionTypes
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound, "NOT_FOUND") { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest, "BAD_REQUEST") { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(message, StatusCodes.Status401Unauthorized, "UNAUTHORIZED") { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message, StatusCodes.Status403Forbidden, "FORBIDDEN") { }
    }

    public class ConflictException : AppException
    {
        protected ConflictException(string message) : base(message, StatusCodes.Status409Conflict, "CONFLICT") { }
    }

    public class DuplicateResourceException : ConflictException
    {
        public DuplicateResourceException(string message) : base(message) { }
    }

    public class ResourceAlreadyExistsException : ConflictException
    {
        public ResourceAlreadyExistsException(string message) : base(message) { }
    }

    public class ValidationException : BadRequestException
    {
        public ValidationException(string message) : base(message) { }
    }

    public class ServerErrorException : AppException
    {
        public ServerErrorException(string message) : base(message, StatusCodes.Status500InternalServerError, "SERVER_ERROR") { }
    }
}