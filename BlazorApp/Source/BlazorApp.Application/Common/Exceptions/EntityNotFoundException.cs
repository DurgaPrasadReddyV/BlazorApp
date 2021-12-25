using System.Net;

namespace BlazorApp.Application.Common.Exceptions;

public class EntityNotFoundException : CustomException
{
    public EntityNotFoundException(string message)
    : base(message, null, HttpStatusCode.NotFound)
    {
    }
}