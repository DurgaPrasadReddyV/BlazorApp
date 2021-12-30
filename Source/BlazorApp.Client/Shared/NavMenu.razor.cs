using BlazorApp.Client.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorApp.Client.Shared;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private ClaimsPrincipal? _currentUser;

    private bool _canViewDashboard;
    private bool _canViewRoles;
    private bool _canViewUsers;

    protected override async Task OnParametersSetAsync()
    {
        var state = await AuthState;
        _currentUser = state.User;
        _canViewDashboard = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Dashboard.View)).Succeeded;
        _canViewRoles = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Roles.View)).Succeeded;
        _canViewUsers = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Users.View)).Succeeded;
    }
}
