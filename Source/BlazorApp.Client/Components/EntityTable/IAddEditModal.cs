using BlazorApp.Client.ApiClient;

namespace BlazorApp.Client.Components.EntityTable;

public interface IAddEditModal
{
    void ForceRender();
    bool Validate(object request);
}