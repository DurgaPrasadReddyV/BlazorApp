using System.Net;
using BlazorApp.Application.Common.Exceptions;

namespace BlazorApp.Application.Identity.Exceptions;

public class IdentityException : CustomException
{
    public IdentityException(string message, List<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, errors, statusCode)
    {
    }
}