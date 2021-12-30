using BlazorApp.Client.ApiClient;

namespace BlazorApp.Client.Components.EntityTable;

public class Result<T> : Result
{
    public T? Data { get; set; }
}

public class ListResult<T> : Result<List<T>>
{
}