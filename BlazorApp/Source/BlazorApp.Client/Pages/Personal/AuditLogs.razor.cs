using BlazorApp.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorApp.Client.Pages.Personal;

public partial class AuditLogs
{
    [Inject]
    private IAuditLogsClient AuditLogsClient { get; set; } = default!;

    public List<RelatedAuditTrail> Trails = new();

    private RelatedAuditTrail _trail = new();
    private string _searchString = string.Empty;
    private bool _dense = true;
    private bool _striped = true;
    private bool _bordered = false;
    private bool _searchInOldValues = false;
    private bool _searchInNewValues = false;
    private MudDateRangePicker _dateRangePicker = default!;
    private DateRange? _dateRange;

    // private ClaimsPrincipal _currentUser;

    // private bool _canExportAuditTrails;
    private bool _canSearchAuditTrails;

    private bool _loaded;

    private bool Search(AuditResponse response)
    {
        bool result = false;

        // check Search String
        if (string.IsNullOrWhiteSpace(_searchString)) result = true;
        if (!result)
        {
            if (response.TableName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }

            if (_searchInOldValues &&
                response.OldValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }

            if (_searchInNewValues &&
                response.NewValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }
        }

        // check Date Range
        if (_dateRange?.Start == null && _dateRange?.End == null) return result;
        if (_dateRange?.Start != null && response.DateTime < _dateRange.Start)
        {
            result = false;
        }

        if (_dateRange?.End != null && response.DateTime > _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))
        {
            result = false;
        }

        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        // _currentUser = await _authService.CurrentUser();

        // _canExportAuditTrails = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Export)).Succeeded;
        _canSearchAuditTrails = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Search)).Succeeded;

        await GetDataAsync();
        _loaded = true;
    }

    private async Task GetDataAsync()
    {
        var response = await AuditLogsClient.GetMyLogsAsync();
        if (response.Succeeded)
        {
            if (response.Data is not null)
            {
                Trails = response.Data
                    .Where(x => x is not null)
                    .Select(x => new RelatedAuditTrail
                    {
                        AffectedColumns = x!.AffectedColumns,
                        DateTime = x.DateTime,
                        Id = x.Id,
                        NewValues = x.NewValues,
                        OldValues = x.OldValues,
                        PrimaryKey = x.PrimaryKey,
                        TableName = x.TableName,
                        Type = x.Type,
                        UserId = x.UserId,
                        LocalTime = DateTime.SpecifyKind(x.DateTime, DateTimeKind.Utc).ToLocalTime()
                    }).ToList();
            }
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private void ShowBtnPress(Guid id)
    {
        _trail = Trails.First(f => f.Id == id);
        foreach (var trial in Trails.Where(a => a.Id != id))
        {
            trial.ShowDetails = false;
        }

        _trail.ShowDetails = !_trail.ShowDetails;
    }

    public class RelatedAuditTrail : AuditResponse
    {
        public bool ShowDetails { get; set; } = false;
        public DateTime LocalTime { get; set; }
    }
}