using System.Net;

namespace BlazorApp.Application.Common.Exceptions;

public class NothingToUpdateException : CustomException
{
    public NothingToUpdateException()
    : base("There are no new changes to update for this Entity.", null, HttpStatusCode.NotAcceptable)
    {
    }
}