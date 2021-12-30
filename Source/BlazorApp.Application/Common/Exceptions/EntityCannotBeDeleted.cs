using System.Net;

namespace BlazorApp.Application.Common.Exceptions;

public class EntityCannotBeDeleted : CustomException
{
    public EntityCannotBeDeleted(string message)
    : base(message, null, HttpStatusCode.Conflict)
    {
    }
}
