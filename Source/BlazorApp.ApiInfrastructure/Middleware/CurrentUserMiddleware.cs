using BlazorApp.Application.Identity.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BlazorApp.CommonInfrastructure.Identity;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;

    public CurrentUserMiddleware(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUser.SetUser(context.User);

        await next(context);
    }
}