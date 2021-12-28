using System.Net;

namespace BlazorApp.Application.Common.Exceptions;

public class EntityAlreadyExistsException : CustomException
{
    public EntityAlreadyExistsException(string message)
    : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}