using Microsoft.AspNetCore.Http;

namespace CustomerOrderSystem.Exceptions;

public class NotFoundException(string message) : ApiException(message, StatusCodes.Status404NotFound);

