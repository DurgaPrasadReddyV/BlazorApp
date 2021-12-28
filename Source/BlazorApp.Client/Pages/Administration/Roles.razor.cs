using BlazorApp.Client.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Client.Pages.Administration;

public partial class Roles
{
    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    private List<RoleDto> _roleList = new();
    private RoleDto _role = new();
    private string _searchString = string.Empty;

    private bool _canCreateRoles;
    private bool _canEditRoles;
    private bool _canDeleteRoles;
    private bool _canSearchRoles;
    private bool _canViewRoleClaims;

    private bool _loaded;

    public bool checkBox { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        _canCreateRoles = true; 
        _canEditRoles = true; 
        _canDeleteRoles = true; 
        _canSearchRoles = true; 
        _canViewRoleClaims = true; 

        await GetRolesAsync();
        _loaded = true;
    }

    private async Task GetRolesAsync()
    {
        var response = await RolesClient.GetListAsync();
        if (response.Succeeded && response.Data is not null)
        {
            _roleList = response.Data.Where(t => t is not null).Cast<RoleDto>().ToList();
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        string deleteContent = _localizer["Delete Content"];
        var parameters = new DialogParameters
            {
                { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var response = await RolesClient.DeleteAsync(id);
            if (response.Succeeded)
            {
                if (response.Messages?.Count > 0)
                {
                    _snackBar.Add(response.Messages.First(), Severity.Success);
                }
            }
            else if (response.Messages is not null)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            await Reset();
        }
    }

    private async Task InvokeModal(string? id = null)
    {
        var parameters = new DialogParameters();
        if (id is not null && _roleList.FirstOrDefault(c => c.Id == id) is RoleDto role)
        {
            _role = role;
            parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
            {
                Id = _role.Id,
                Name = _role.Name,
                Description = _role.Description
            });
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<RoleModal>(id == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _role = new RoleDto();
        await GetRolesAsync();
    }

    private bool Search(RoleDto role)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private void ManagePermissions(string? roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            throw new ArgumentNullException(nameof(roleId));
        }

        _navigationManager.NavigateTo($"/identity/role-permissions/{roleId}");
    }
}