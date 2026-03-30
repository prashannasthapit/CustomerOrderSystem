using Microsoft.AspNetCore.Http;

namespace CustomerOrderSystem.Exceptions;

public class ConflictException(string message) : ApiException(message, StatusCodes.Status409Conflict);

